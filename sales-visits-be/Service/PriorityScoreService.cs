using entities;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models;

namespace sales_visits_be.Service;

public class PriorityScoreService
{
    private readonly ApplicationDbContext _db;
    private readonly TerritoryService _territoryService;
    private const double RecencyWeight = 0.5;
    private const double FrequencyWeight = 0.3;
    private const double ConsistencyWeight = 0.2;

    public PriorityScoreService(ApplicationDbContext db, TerritoryService territoryService)
    {
        _db = db;
        _territoryService = territoryService;
    }

    public async Task<List<CustomerPriorityScore>> GetScoredCustomersAsync(Guid salesId, int topN = 10,
        CancellationToken cancellationToken = default)
    {
        var territory = await _territoryService.GetTerritoryAsync(salesId, cancellationToken);
        var territoryCustomerIds = territory.Customers.Select(q => q.Id).ToHashSet();

        var visitGroups = await _db.SalesVisits
            .Where(q => q.SalesId == salesId &&
                        territoryCustomerIds.Contains(q.CustomerLocationId))
            .Include(q => q.CustomerLocation)
            .GroupBy(q => q.CustomerLocationId)
            .Select(q => new
            {
                CustomerLocationId = q.Key,
                CustomerLocation = q.First().CustomerLocation,
                VisitDates = q.Select(v => v.CreatedAt).OrderBy(v => v).ToList(),
                VisitCount = q.Count(),
                LastVisitedAt = q.Max(v => v.CreatedAt)
            })
            .ToListAsync(cancellationToken);
        if (!visitGroups.Any())
        {
            return new List<CustomerPriorityScore>();
        }

        int maxVisitCount = visitGroups.Max(q => q.VisitCount);
        var now = DateTime.UtcNow;
        var today = DateTime.UtcNow.Date;

        var visitedTodayIds = await _db.SalesVisits
            .Where(q => q.SalesId == salesId && q.CreatedAt >= today)
            .Select(q => q.CustomerLocationId)
            .ToListAsync(cancellationToken);
        
        var scored = visitGroups.Select(q =>
            {
                double avgDaysBetweenVisits = ComputeAvgGap(q.VisitDates);
                double daysSinceLast = (now - q.LastVisitedAt).TotalDays;

                double recencyScore =
                    avgDaysBetweenVisits > 0 ? Math.Min(daysSinceLast / avgDaysBetweenVisits, 1.0) : 1.0;
                double frequencyScore = maxVisitCount > 0 ? (double)q.VisitCount / maxVisitCount : 0;
                double consistencyScore = ComputeConsistencyScore(q.VisitDates);

                double priority = (recencyScore * RecencyWeight) +
                                  (frequencyScore * FrequencyWeight) +
                                  (consistencyScore * ConsistencyWeight);

                return new CustomerPriorityScore
                {
                    CustomerLocationId = q.CustomerLocationId,
                    CustomerName = q.CustomerLocation.Name,
                    Latitude = q.CustomerLocation.Location.Y,
                    Longitude = q.CustomerLocation.Location.X,
                    PriorityScore = Math.Round(priority, 4),
                    RecencyScore = Math.Round(recencyScore, 4),
                    ConsistencyScore = Math.Round(consistencyScore, 4),
                    FrequencyScore = Math.Round(frequencyScore, 4),
                    LastVisitedAt = q.LastVisitedAt
                };
            })
            .OrderByDescending(q => q.PriorityScore)
            .Where(q => !visitedTodayIds.Contains(q.CustomerLocationId))
            .Take(topN).ToList();

        return scored;
    }

    private static double ComputeAvgGap(List<DateTime> sortedDates)
    {
        if (sortedDates.Count < 2)
        {
            return 0;
        }

        var gaps = sortedDates.Zip(sortedDates.Skip(1), (a, b) => (b - a).TotalDays).ToList();
        return gaps.Average();
    }

    private static double ComputeConsistencyScore(List<DateTime> sortedDates)
    {
        if (sortedDates.Count < 3)
        {
            return 0.5;
        }

        var gaps = sortedDates.Zip(sortedDates.Skip(1), (a, b) => (b - a).TotalDays).ToList();
        double mean = gaps.Average();
        double stdDev = Math.Sqrt(gaps.Average(q => Math.Pow(q - mean, 2)));
        if (mean == 0)
        {
            return 1.0;
        }

        double cv = stdDev / mean;
        return Math.Max(0, Math.Min(1.0 - cv, 1.0));
    }
}
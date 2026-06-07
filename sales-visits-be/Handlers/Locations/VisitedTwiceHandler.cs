using entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class VisitedTwiceHandler : IRequestHandler<VisitedTwiceRequest, VisitedTwiceResponse>
{
    private readonly ApplicationDbContext _db;

    public VisitedTwiceHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<VisitedTwiceResponse> Handle(VisitedTwiceRequest request, CancellationToken cancellationToken)
    {
        var startOfMonth = new DateTime(request.Year, request.Month, 1).ToUniversalTime();
        var endOfMonth = startOfMonth.AddMonths(1);
        var query = _db.SalesVisits
            .Where(q =>
                q.SalesId == request.SalesId &&
                q.CreatedAt >= startOfMonth &&
                q.CreatedAt < endOfMonth
            ).AsQueryable();
        
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.ItemsPerPage <= 0 ? 10 : request.ItemsPerPage;
        var skip = (page - 1) * pageSize;
        
        var allLocationVisitCounts = await query
            .GroupBy(q => q.CustomerLocationId)
            .Select( g => new
            {
                CustomerLocationId = g.Key,
                VisitCount = g.Count(),
                FirstVisit = g.Min(q => q.CreatedAt),
                LastVisit = g.Max(q => q.CreatedAt)
            }).ToListAsync(cancellationToken);
        
        var totalData = allLocationVisitCounts
            .Count(q => q.VisitCount >= 2 && (q.LastVisit - q.FirstVisit).TotalDays >= 7);

        var locationVisitCounts = allLocationVisitCounts
            .Where(q => q.VisitCount >= 2 && (q.LastVisit - q.FirstVisit).TotalDays >= 7)
            .OrderByDescending(q => q.LastVisit)
            .Skip(skip)
            .Take(pageSize)
            .ToList();

        var visitedTwiceList = new List<VisitedTwiceData>();
        var visitedWeeklyIds = locationVisitCounts
            .Where(q => q.VisitCount >= 2 && (q.LastVisit - q.FirstVisit).TotalDays >= 7)
            .Select(q => q.CustomerLocationId).ToList();

        var visitedTwiceLocations = await _db.CustomerLocations
            .Where(q => visitedWeeklyIds.Contains(q.Id))
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync(cancellationToken);

        foreach (var location in visitedTwiceLocations)
        {
            var locationGroup = locationVisitCounts.FirstOrDefault(q => q.CustomerLocationId == location.Id);
            var oldest = locationGroup.FirstVisit;
            var newest = locationGroup.LastVisit;
            visitedTwiceList.Add(new VisitedTwiceData
            {
                Name = location.Name,
                Address = location.Address,
                NewestVisit = newest,
                OldestVisit = oldest,
            });
        }
        return new VisitedTwiceResponse
        {
            Locations = visitedTwiceList,
            TotalData = totalData
        };
    }
}
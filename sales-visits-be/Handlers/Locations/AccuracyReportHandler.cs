using entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class AccuracyReportHandler : IRequestHandler<AccuracyReportRequest, AccuracyReportResponse>
{
    private readonly ApplicationDbContext _db;

    public AccuracyReportHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<AccuracyReportResponse> Handle(AccuracyReportRequest request, CancellationToken cancellationToken)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var startOfMonth = new DateTime(request.Year, request.Month, 1).ToUniversalTime();
        var startOfNextMonth = startOfMonth.AddMonths(1);
        var visits = await _db.SalesVisits
            .Include(q => q.CustomerLocation)
            .Where(q => q.SalesId == request.SalesId &&
                        q.CreatedAt >= startOfMonth &&
                        q.CreatedAt < startOfNextMonth)
            .OrderBy(q => q.CreatedAt)
            .ToListAsync(cancellationToken);

        var plans = await _db.VisitPlans
            .Include(q => q.CustomerLocation)
            .Where(q => q.SalesId == request.SalesId &&
                        q.Date >= startOfMonth &&
                        q.Date < startOfNextMonth)
            .OrderBy(q => q.VisitOrder)
            .ToListAsync(cancellationToken);
        var visitGroups = visits.GroupBy(q => TimeZoneInfo.ConvertTimeFromUtc(q.CreatedAt, timeZone).Date).ToList();
        var planGroups = plans.GroupBy(q => TimeZoneInfo.ConvertTimeFromUtc(q.Date.ToUniversalTime(), timeZone).Date)
            .ToList();
        var list = new List<AccuracyReport>();
        foreach (var visitedGroup in visitGroups)
        {
            var date = visitedGroup.Key.Date;
            var planGroup = planGroups.FirstOrDefault(q => q.Key == date);
            var visited = visitedGroup.OrderBy(q => q.CreatedAt).Select(q => new VisitedLocationData
            {
                Name = q.CustomerLocation.Name,
                Note = q.Note,
                Time = TimeOnly.FromDateTime(q.CreatedAt)
            }).ToList();
            var planList = new List<string>();
            if (planGroup != null)
            {
                var visitPlans = planGroup.OrderBy(q => q.Date).Select(q => q.CustomerLocation.Name).ToList();
                planList.AddRange(visitPlans);
            }

            var matching = visitedGroup.Count(q => planList.Contains(q.CustomerLocation.Name));
            list.Add(new AccuracyReport
            {
                Accuracy = planList.Count > 0 ? Math.Round((double)matching / planList.Count * 100, 2) : null,
                Date = date,
                PlannedLocations = planList,
                VisitedLocations = visited
            });
        }
        var visitGroupDates = visitGroups.Select(q => q.Key.Date).ToList();
        var onlyPlanned = planGroups.Where(q => !visitGroupDates.Contains(q.Key.Date)).ToList();
        foreach(var planGroup in onlyPlanned)
        {
            var date = planGroup.Key.Date;
            var planList = new List<string>();
            var visitPlans = planGroup.OrderBy(q => q.Date).Select(q => q.CustomerLocation.Name).ToList();
            planList.AddRange(visitPlans);
            list.Add(new AccuracyReport
            {
                Accuracy = null,
                Date = date,
                PlannedLocations = planList,
            });
            
        }
        
        list = list.OrderBy(q => q.Date).ToList();

        var firstDayOfMonth = TimeZoneInfo.ConvertTimeFromUtc(startOfMonth, timeZone);
        var weeklyReports = list.GroupBy(q =>
            {
                var offset = (int)firstDayOfMonth.DayOfWeek;
                var day = q.Date.Day;
                return ((day + offset - 1) / 7) + 1;
            }).OrderBy(q => q.Key)
            .Select(q => new WeeklyReport
            {
                Week = q.Key,
                Reports = q.OrderBy(r => r.Date).ToList()
            }).ToList();
        return new AccuracyReportResponse
        {
            WeeklyReports = weeklyReports
        };
    }
}
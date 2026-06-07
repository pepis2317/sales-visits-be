using entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class
    GetMonthlyVisitedLocationsHandler : IRequestHandler<GetMonthlyVisitedLocationsRequest,
    GetMonthlyVisitedLocationsResponse>
{
    private readonly ApplicationDbContext _db;

    public GetMonthlyVisitedLocationsHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<GetMonthlyVisitedLocationsResponse> Handle(GetMonthlyVisitedLocationsRequest request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var startOfMonth = new DateTime(today.Year, today.Month, 1).ToUniversalTime();
        var startOfNextMonth = startOfMonth.AddMonths(1);
        var raw = await _db.SalesVisits
            .Include(q => q.VisitType)
            .Include(q => q.CustomerLocation)
            .Where(q => q.SalesId == request.SalesId &&
                        q.CreatedAt >= startOfMonth &&
                        q.CreatedAt < startOfNextMonth)
            .ToListAsync(cancellationToken);
        var grouped = raw.GroupBy(q =>
        {
            var firstDayOfMonth = startOfMonth;
            var offset = (int)firstDayOfMonth.DayOfWeek;
            var day = q.CreatedAt.Day;
            return ((day + offset - 1) / 7) + 1;
        }).Select(q => new MonthlyVisitedData
        {
            Week = q.Key,
            WeeklyLocations = q
                .GroupBy(w => w.CreatedAt.Date)
                .Select(w => new WeeklyVisitedData
                {
                    Day = (int)w.Key.DayOfWeek,
                    Locations = w.OrderBy(d => d.CreatedAt)
                        .Select(d => new VisitedData
                        {
                            Name = d.CustomerLocation.Name,
                            Address = d.CustomerLocation.Address,
                            VisitedAt =  d.CreatedAt,
                            Note = d.Note ?? d.VisitType.Name,
                        }).ToList()
                }).OrderBy(q => q.Day).ToList()
        }).OrderBy(q=>q.Week).ToList();

        return new GetMonthlyVisitedLocationsResponse
        {
            MonthlyLocations = grouped
        };
    }
}
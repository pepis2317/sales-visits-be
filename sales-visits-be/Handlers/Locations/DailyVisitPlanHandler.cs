using entities;
using entities.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class DailyVisitPlanHandler:IRequestHandler<DailyVisitPlanRequest, DailyVisitPlanResponse>
{
    private readonly ApplicationDbContext _db;
    public DailyVisitPlanHandler(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<DailyVisitPlanResponse> Handle(DailyVisitPlanRequest request, CancellationToken cancellationToken)
    {
        var userPoint = new Point(request.Longitude, request.Latitude) { SRID = 4326 };
        var todayStart = DateTime.Today.ToUniversalTime();
        var todayEnd = todayStart.AddDays(1);
        var list = new List<LocationData>();
        var visitedNames= await _db.SalesVisits
            .Include(q => q.CustomerLocation)
            .Where(q => q.SalesId == request.SalesId && 
                        q.CreatedAt >= todayStart && 
                        q.CreatedAt < todayEnd)
            .OrderBy(q => q.CreatedAt)
            .Select(q => q.CustomerLocation.Name).ToListAsync(cancellationToken);
        
        var plans = await _db.VisitPlans
            .Include(q => q.CustomerLocation)
            .Where(q => q.SalesId == request.SalesId && 
                        q.Date >= todayStart&&
                        q.Date < todayEnd)
            .OrderBy(q => q.VisitOrder)
            .Select(q => new LocationData
            {
                Name = q.CustomerLocation.Name,
                Address =  q.CustomerLocation.Address,
                LastVisitedAt =  q.CustomerLocation.LastVisitedAt,
                ApproximateDistance = Math.Round(q.CustomerLocation.Location.Distance(userPoint) * 111.139, 2)
            }).ToListAsync(cancellationToken);
        
        var duePlans = await _db.VisitPlans
            .Where(q => q.Date >= todayStart.AddDays(-14) && 
                        q.Date < todayEnd.AddDays(-14) &&
                        q.SalesId == request.SalesId)
            .OrderBy(q => q.VisitOrder)
            .Select(q => new LocationData
            {
                Name = q.CustomerLocation.Name,
                Address =  q.CustomerLocation.Address,
                LastVisitedAt =  q.CustomerLocation.LastVisitedAt,
                ApproximateDistance = Math.Round(q.CustomerLocation.Location.Distance(userPoint) * 111.139, 2)
            }).ToListAsync(cancellationToken);
        var planNames = plans.Select(q=>q.Name).ToList();
        var excluded = duePlans.Where(q=>!planNames.Contains(q.Name)).ToList();
        list.AddRange(plans);
        list.AddRange(excluded);
        list.RemoveAll(q => visitedNames.Contains(q.Name));
        return new DailyVisitPlanResponse
        {
            Locations = list,
            Visited = visitedNames
        };
    }
}
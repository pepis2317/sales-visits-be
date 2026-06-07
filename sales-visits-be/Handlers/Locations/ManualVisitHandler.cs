using entities;
using entities.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class ManualVisitHandler:IRequestHandler<ManualVisitRequest, LocationResponse>
{
    private readonly ApplicationDbContext _db;
    public ManualVisitHandler(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<LocationResponse> Handle(ManualVisitRequest request, CancellationToken cancellationToken)
    {
        var location = await _db.CustomerLocations.FirstOrDefaultAsync(q => q.Id == request.LocationId, cancellationToken);
        if(location == null || location.Location == null)
        {
            return new LocationResponse
            {
                IsSuccess = false,
                Message = "Location not found"
            };
        }

        var visitTime = request.DateTime.ToUniversalTime().AddHours(-1);
        location.LastVisitedAt = visitTime;
        var visit = new SalesVisit
        {
            CustomerLocationId = location.Id,
            SalesId = request.SalesId,
            CreatedAt =  visitTime,
            Note = request.Note
        };
        
        _db.SalesVisits.Add(visit);
        await _db.SaveChangesAsync(cancellationToken);
        return new LocationResponse
        {
            IsSuccess = true,
            Message = $"{location.Name} has been visited"
        };
    }
}
using entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class GetLocationsListHandler: IRequestHandler<GetLocationsListRequest, GetLocationsListResponse>
{
    private readonly ApplicationDbContext _db;
    public GetLocationsListHandler(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<GetLocationsListResponse> Handle(GetLocationsListRequest request, CancellationToken cancellationToken)
    {
        var userPoint = new Point(request.Longitude, request.Latitude) { SRID = 4326 };
        var nearestList = await _db.CustomerLocations.OrderBy(q => q.Location.Distance(userPoint))
            .Take(10)
            .Select(q => new LocationData
            {
                Name = q.Name,
                Address = q.Address,
                LastVisitedAt = q.LastVisitedAt,
                ApproximateDistance = Math.Round(q.Location.Distance(userPoint) * 111.139, 2) 
            }).ToListAsync(cancellationToken);
        
        return new GetLocationsListResponse
        {
            Locations = nearestList
        };
    }
}
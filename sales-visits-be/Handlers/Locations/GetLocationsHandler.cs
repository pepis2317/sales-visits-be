using entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using sales_visits_be.Models;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class GetLocationsHandler : IRequestHandler<GetLocationsRequest, GetLocationsResponse>
{
    private readonly ApplicationDbContext _db;

    public GetLocationsHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<GetLocationsResponse> Handle(GetLocationsRequest request, CancellationToken cancellationToken)
    {
        var userPoint = new Point(request.Longitude, request.Latitude) { SRID = 4326 };
        var query = _db.CustomerLocations.AsQueryable();
        if (!string.IsNullOrEmpty(request.Query))
        {
            var normalizedQuery = request.Query.ToLower();

            var list = await query
                .Where(q => q.Name.ToLower().Contains(normalizedQuery) ||
                            EF.Functions.TrigramsSimilarity(q.Name.ToLower(), normalizedQuery) > 0.1)
                .OrderByDescending(q => EF.Functions.TrigramsSimilarity(q.Name.ToLower(), normalizedQuery))
                .Take(10)
                .Select(q => new LocationDropdown
                {
                    Label = q.Name,
                    Value = q.Id.ToString(),
                    ApproximateDistance = Math.Round(q.Location.Distance(userPoint) * 111.139, 2)
                }).ToListAsync(cancellationToken);

            return new GetLocationsResponse { Locations = list };
        }

        var nearestList = await query.OrderBy(q => q.Location.Distance(userPoint))
            .Take(10)
            .Select(q => new LocationDropdown
            {
                Label = q.Name,
                Value = q.Id.ToString(),
                ApproximateDistance = Math.Round(q.Location.Distance(userPoint) * 111.139, 2)
            }).ToListAsync(cancellationToken);
        return new GetLocationsResponse
        {
            Locations = nearestList
        };
    }
}
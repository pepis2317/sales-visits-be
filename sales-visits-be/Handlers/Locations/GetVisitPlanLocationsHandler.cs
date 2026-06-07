using System.Drawing;
using entities;
using MediatR;
using sales_visits_be.Models.Locations;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models;

namespace sales_visits_be.Handlers.Locations;

public class GetVisitPlanLocationsHandler : IRequestHandler<GetVisitPlanLocationsRequest, GetVisitPlanLocationsResponse>
{
    private readonly ApplicationDbContext _db;

    public GetVisitPlanLocationsHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<GetVisitPlanLocationsResponse> Handle(GetVisitPlanLocationsRequest request, CancellationToken cancellationToken)
    {
        Dropdown? initial = null; 
        var query = _db.CustomerLocations.AsQueryable();
        if (!string.IsNullOrEmpty(request.Query))
        {
            var normalizedQuery = request.Query.ToLower();

            var list = await query
                .Where(q => q.Name.ToLower().Contains(normalizedQuery) ||
                            EF.Functions.TrigramsSimilarity(q.Name.ToLower(), normalizedQuery) > 0.1)
                .OrderByDescending(q => EF.Functions.TrigramsSimilarity(q.Name.ToLower(), normalizedQuery))
                .Take(10)
                .Select(q => new Dropdown
                {
                    Label = q.Name,
                    Value = q.Id.ToString(),
                }).ToListAsync(cancellationToken);
            return new GetVisitPlanLocationsResponse{Locations = list};
        }
        if (request.LastLocationId != null)
        {
            var lastLocation = await _db.CustomerLocations
                .FirstOrDefaultAsync(q => q.Id == request.LastLocationId, cancellationToken);
            if (lastLocation != null)
            {
                var list = await query.OrderBy(q => q.Location.Distance(lastLocation.Location))
                    .Take(10)
                    .Select(q => new Dropdown
                    {
                        Label = q.Name,
                        Value = q.Id.ToString(),
                    }).ToListAsync(cancellationToken);
                return new GetVisitPlanLocationsResponse{Locations = list};
            }
        }
        var locationsList = await query.OrderBy(q => q.Name)
            .Take(10)
            .Select(q => new Dropdown
            {
                Label = q.Name,
                Value = q.Id.ToString(),
            }).ToListAsync(cancellationToken);
        return new GetVisitPlanLocationsResponse
        {
            Locations = locationsList,
            InitialValue = initial
            
        };
    }
}
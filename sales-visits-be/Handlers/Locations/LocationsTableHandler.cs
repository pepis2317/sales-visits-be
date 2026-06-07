using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using entities;
using entities.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class LocationsTableHandler : IRequestHandler<LocationsTableRequest, LocationsTableResponse>
{
    private readonly ApplicationDbContext _db;

    public LocationsTableHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<LocationsTableResponse> Handle(LocationsTableRequest request, CancellationToken cancellationToken)
    {
        var query = _db.CustomerLocations.AsQueryable();

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.ItemsPerPage <= 0 ? 10 : request.ItemsPerPage;
        var skip = (page - 1) * pageSize;
        if (!string.IsNullOrEmpty(request.Query))
        {
            var normalizedQuery = request.Query.ToLower();
            var filtered = query.Where(q =>
                q.Name.ToLower().Contains(normalizedQuery) ||
                EF.Functions.TrigramsSimilarity(q.Name.ToLower(), normalizedQuery) > 0.1);
            var totalData = await filtered.CountAsync(cancellationToken);
            IQueryable<CustomerLocation> orderedQuery = filtered.OrderByDescending(q =>
                EF.Functions.TrigramsSimilarity(q.Name.ToLower(), normalizedQuery));
            var list = await orderedQuery
                .Skip(skip)
                .Take(pageSize)
                .Select(q => new LocationsTableData
                {
                    Id = q.Id,
                    Name = q.Name,
                    Address = q.Address,
                    Note = q.Note,
                    Potential = q.Potential ?? 0
                }).ToListAsync(cancellationToken);

            return new LocationsTableResponse
            {
                Locations = list,
                TotalData = totalData
            };
        }

        var totalDataUnfiltered = await query.CountAsync(cancellationToken);
        var listUnfiltered = await query
            .OrderBy(q => q.Name)
            .Skip(skip)
            .Take(pageSize)
            .Select(q => new LocationsTableData
            {
                Id = q.Id,
                Name = q.Name,
                Address = q.Address,
                Note = q.Note,
                Potential = q.Potential ?? 0
            }).ToListAsync(cancellationToken);

        return new LocationsTableResponse
        {
            Locations = listUnfiltered,
            TotalData = totalDataUnfiltered
        };
    }
}
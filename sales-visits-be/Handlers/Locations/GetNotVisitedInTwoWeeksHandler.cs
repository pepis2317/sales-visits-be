using entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class
    GetNotVisitedInTwoWeeksHandler : IRequestHandler<GetNotVisitedInTwoWeeksRequest, GetNotVisitedInTwoWeeksResponse>
{
    private readonly ApplicationDbContext _db;

    public GetNotVisitedInTwoWeeksHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<GetNotVisitedInTwoWeeksResponse> Handle(GetNotVisitedInTwoWeeksRequest request,
        CancellationToken cancellationToken)
    {
        var visitedIds = await _db.SalesVisits.Where(q => q.SalesId == request.SalesId)
            .Select(q => q.CustomerLocationId).Distinct().ToListAsync(cancellationToken);
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.ItemsPerPage <= 0 ? 10 : request.ItemsPerPage;
        var skip = (page - 1) * pageSize;
        var query = _db.CustomerLocations
            .Where(q => visitedIds.Contains(q.Id) && q.LastVisitedAt <= DateTime.UtcNow.AddDays(-14))
            .AsQueryable();
        var locations = await query
            .OrderBy(q => q.LastVisitedAt)
            .Skip(skip)
            .Take(pageSize)
            .Select(q => new NotVisitedData
            {
                Name = q.Name,
                Address = q.Address,
                LastVisitedAt = q.LastVisitedAt
            }).ToListAsync(cancellationToken);
        var totalData = await query.CountAsync(cancellationToken);
        return new GetNotVisitedInTwoWeeksResponse
        {
            Locations = locations,
            TotalData = totalData
        };
    }
}
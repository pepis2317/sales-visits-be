using entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class RepositionListHandler : IRequestHandler<RepositionListRequest, RepositionListResponse>
{
    private readonly ApplicationDbContext _db;

    public RepositionListHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<RepositionListResponse> Handle(RepositionListRequest request, CancellationToken cancellationToken)
    {
        var query = _db.RepositionRequests.Where(q => q.AcceptedAt == null && q.DeclinedAt == null).AsQueryable();
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.ItemsPerPage <= 0 ? 10 : request.ItemsPerPage;
        var skip = (page - 1) * pageSize;

        var list = await query
            .OrderByDescending(q => q.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .Select(q => new RepositionData
            {
                Id = q.Id,
                Name = q.CustomerLocation.Name,
                NewAddress = q.Address,
                OldAddress = q.CustomerLocation.Address,
                Sales = q.Sales.Name
            }).ToListAsync(cancellationToken);
        var totalData = await query.CountAsync(cancellationToken);
        return new RepositionListResponse
        {
            Requests = list,
            TotalData = totalData
        };
    }
}
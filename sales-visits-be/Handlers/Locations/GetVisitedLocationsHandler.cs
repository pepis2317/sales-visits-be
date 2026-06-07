using entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class GetVisitedLocationsHandler:IRequestHandler<GetVisitedLocationsRequest,GetVisitedLocationsResponse>
{
    private readonly ApplicationDbContext _db;
    public GetVisitedLocationsHandler(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<GetVisitedLocationsResponse> Handle(GetVisitedLocationsRequest request, CancellationToken cancellationToken)
    {
        var query = _db.SalesVisits.AsQueryable();
        if(request.SalesId != null)
        {
            query = query.Where(q => q.SalesId == request.SalesId);
        }
        if(request.TypeId != null)
        {
            query = query.Where(q => q.VisitTypeId == request.TypeId);
        }
        if(request.Year != null)
        {
            query = query.Where(q => q.CreatedAt.Year == request.Year);
        }
        if(request.Month != null)
        {
            query = query.Where(q => q.CreatedAt.Month == request.Month);
        }

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.ItemsPerPage <= 0 ? 10 : request.ItemsPerPage;
        var skip = (page - 1) * pageSize;

        var list = await query
            .OrderByDescending(q => q.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .Select(q => new VisitedListData
            {
                CustomerName = q.CustomerLocation.Name,
                Sales = q.Sales.Name,
                Note = q.Note ?? q.VisitType.Name,
                VisitedAt = q.CreatedAt
            }).ToListAsync(cancellationToken);
        
        var totalData = await query.CountAsync(cancellationToken);
        return new GetVisitedLocationsResponse
        {
            VisitedData = list,
            TotalData = totalData
        };
    }
}
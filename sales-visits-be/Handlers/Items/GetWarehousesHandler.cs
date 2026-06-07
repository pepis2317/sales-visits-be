using entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models;
using sales_visits_be.Models.Items;

namespace sales_visits_be.Handlers.Items;

public class GetWarehousesHandler:IRequestHandler<GetWarehousesRequest, GetWarehousesResponse>
{
    private readonly ApplicationDbContext _db;
    public GetWarehousesHandler(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<GetWarehousesResponse> Handle(GetWarehousesRequest request, CancellationToken cancellationToken)
    {
        var warehouses = await _db.Warehouses.Select(q => new Dropdown
        {
            Label = q.Name,
            Value = q.Id.ToString()
        }).ToListAsync(cancellationToken);
        return new GetWarehousesResponse {Warehouses = warehouses};
    }
}
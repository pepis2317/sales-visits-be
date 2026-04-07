using entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models;
using sales_visits_be.Models.Sales;

namespace sales_visits_be.Handlers.Sales;

public class GetSalesHandler:IRequestHandler<GetSalesRequest, GetSalesResponse>
{
    private readonly ApplicationDbContext _db;
    public GetSalesHandler(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<GetSalesResponse> Handle(GetSalesRequest request, CancellationToken cancellationToken)
    {
        var sales = await _db.Sales.Select(q => new Dropdown{Value = q.Id.ToString(), Label = q.Name}).ToListAsync(cancellationToken);
        return new GetSalesResponse{Sales = sales};
    }
}
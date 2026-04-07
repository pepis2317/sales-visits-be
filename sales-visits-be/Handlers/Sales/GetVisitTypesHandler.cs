using entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models;
using sales_visits_be.Models.VisitTypes;

namespace sales_visits_be.Handlers.Sales;

public class GetVisitTypesHandler:IRequestHandler<GetVisitTypesRequest, GetVisitTypesResponse>
{
    private readonly ApplicationDbContext _db;
    public GetVisitTypesHandler(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<GetVisitTypesResponse> Handle(GetVisitTypesRequest request, CancellationToken cancellationToken)
    {
        var types = await _db.VisitTypes.Select(q => new Dropdown{Value = q.Id.ToString(), Label = q.Name}).ToListAsync(cancellationToken);
        return new GetVisitTypesResponse{Types = types};
    }
}
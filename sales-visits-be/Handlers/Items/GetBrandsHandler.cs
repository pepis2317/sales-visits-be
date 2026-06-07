using entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models;
using sales_visits_be.Models.Items;

namespace sales_visits_be.Handlers.Items;

public class GetBrandsHandler:IRequestHandler<GetBrandsRequest, GetBrandsResponse>
{
    private readonly ApplicationDbContext _db;
    public GetBrandsHandler(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<GetBrandsResponse> Handle(GetBrandsRequest request, CancellationToken cancellationToken)
    {
        var brands = await _db.Brands.Select(q => new Dropdown
        {
            Label = q.Name,
            Value = q.Id.ToString()
        }).ToListAsync(cancellationToken);
        return new GetBrandsResponse
        {
            Brands = brands
        };
    }
}
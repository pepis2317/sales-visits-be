using entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class GetVisitPlanDayHandler : IRequestHandler<GetVisitPlanDayRequest, GetVisitPlanDayResponse>
{
    private readonly ApplicationDbContext _db;

    public GetVisitPlanDayHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<GetVisitPlanDayResponse> Handle(GetVisitPlanDayRequest dayRequest, CancellationToken cancellationToken)
    {
        var startOfDay = dayRequest.Date.ToUniversalTime();
        var endOfDay = dayRequest.Date.AddDays(1).ToUniversalTime();
        var visitPlans = await _db.VisitPlans
            .Include(q => q.CustomerLocation)
            .Where(q => q.SalesId == dayRequest.SalesId && q.Date >= startOfDay && q.Date < endOfDay)
            .OrderBy(q => q.Date).ThenBy(q => q.VisitOrder)
            .Select(q => new VisitPlanData
            {
                LocationId = q.CustomerLocationId,
                VisitOrder = q.VisitOrder,
                LocationName = q.CustomerLocation.Name,
                Date =  q.Date,
            })
            .ToListAsync(cancellationToken);
        return new GetVisitPlanDayResponse
        {
            Plans = visitPlans
        };
    }
}
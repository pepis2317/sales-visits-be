using entities;
using entities.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class CreateVisitPlanHandler : IRequestHandler<CreateVisitPlanRequest, LocationResponse>
{
    private readonly ApplicationDbContext _db;

    public CreateVisitPlanHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<LocationResponse> Handle(CreateVisitPlanRequest request, CancellationToken cancellationToken)
    {
        var startOfDay = request.Date.ToUniversalTime();
        var endOfDay = request.Date.AddDays(1).ToUniversalTime();
        var plan = await _db.VisitPlans
            .FirstOrDefaultAsync(q => q.Date >= startOfDay &&
                                      q.Date <= endOfDay &&
                                      q.SalesId == request.SalesId &&
                                      q.VisitOrder == request.VisitOrder, cancellationToken);
        if(plan == null)
        {
            _db.VisitPlans.Add(new VisitPlan
            {
                Date = request.Date,
                SalesId = request.SalesId,
                CustomerLocationId = request.LocationId,
                VisitOrder = request.VisitOrder,
                CreatedAt =  DateTime.UtcNow,
            });
        }
        else
        {
            plan.CustomerLocationId = request.LocationId;
            plan.UpdatedAt = DateTime.UtcNow;
        }
        
        await _db.SaveChangesAsync(cancellationToken);

        return new LocationResponse
        {
            IsSuccess = true,
            Message = "Successfully created visit plan"
        };
    }
}
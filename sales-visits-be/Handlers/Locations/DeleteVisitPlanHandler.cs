using entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class DeleteVisitPlanHandler : IRequestHandler<DeleteVisitPlanRequest, LocationResponse>
{
    private readonly ApplicationDbContext _db;

    public DeleteVisitPlanHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<LocationResponse> Handle(DeleteVisitPlanRequest request, CancellationToken cancellationToken)
    {
        var startOfDay = request.Date.ToUniversalTime();
        var endOfDay = request.Date.AddDays(1).ToUniversalTime();
        var plan = await _db.VisitPlans
            .FirstOrDefaultAsync(q => q.Date >= startOfDay  && 
                                      q.Date <= endOfDay && 
                                      q.SalesId == request.SalesId &&
                                      q.VisitOrder == request.VisitOrder, cancellationToken);
        if(plan == null)
        {
            return new LocationResponse
            {
                IsSuccess = false,
                Message = "Plan not found"
            };
        }
        _db.VisitPlans.Remove(plan);
        await _db.SaveChangesAsync(cancellationToken);
        return new LocationResponse
        {
            IsSuccess = true,
            Message = "Successfully removed plan"
        };
    }
}
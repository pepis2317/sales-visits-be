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

    private static DateTime ToGmt7UtcMidnight(DateOnly date)
    {
        var gmt7Zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        return TimeZoneInfo.ConvertTimeToUtc(
            new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Unspecified),
            gmt7Zone
        );
    }

    public async Task<LocationResponse> Handle(CreateVisitPlanRequest request, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        if (request.Id != null)
        {
            var plan = await _db.VisitPlans.FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);
            if (plan == null)
            {
                return new LocationResponse
                {
                    IsSuccess = false,
                    Message = "No plan found"
                };
            }
            plan.CustomerLocationId = request.LocationId;
            plan.UpdatedAt = DateTime.UtcNow;
            id =(Guid) request.Id;
        }
        else
        {
            var utcMidnight = ToGmt7UtcMidnight(request.Date);
            _db.VisitPlans.Add(new VisitPlan
            {
                Id = id,
                Date = utcMidnight,
                SalesId = request.SalesId,
                CustomerLocationId = request.LocationId,
                VisitOrder = request.VisitOrder,
                CreatedAt = DateTime.UtcNow,
            });
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new LocationResponse
        {
            IsSuccess = true,
            Message = "Successfully created visit plan",
            Id = id
        };
    }
}
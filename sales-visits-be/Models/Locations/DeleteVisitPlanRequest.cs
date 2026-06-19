using MediatR;

namespace sales_visits_be.Models.Locations;

public class DeleteVisitPlanRequest:IRequest<LocationResponse>
{
    public Guid Id { get; set; }
}
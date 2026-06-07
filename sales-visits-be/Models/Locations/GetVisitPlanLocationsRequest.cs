using MediatR;

namespace sales_visits_be.Models.Locations;

public class GetVisitPlanLocationsRequest:IRequest<GetVisitPlanLocationsResponse>
{
    public Guid? LastLocationId { get; set; }
    public string? Query { get; set; }
}
using MediatR;

namespace sales_visits_be.Models.Locations;

public class RespondRepositionRequest:IRequest<LocationResponse>
{
    public Guid RepositionRequestId { get; set; }
    public bool IsApproved { get; set; }
}
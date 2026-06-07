using MediatR;

namespace sales_visits_be.Models.Locations;

public class ManualVisitRequest:IRequest<LocationResponse>
{
    public Guid SalesId { get; set; }
    public Guid LocationId { get; set; }
    public DateTime DateTime { get; set; }
    public string Note { get; set; } = string.Empty;
}
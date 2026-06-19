using MediatR;

namespace sales_visits_be.Models.Locations;

public class CreateVisitPlanRequest:IRequest<LocationResponse>
{
    public Guid? Id { get; set; }
    public Guid SalesId { get; set; }
    public Guid LocationId { get; set; }
    public DateOnly Date { get; set; }
    public int VisitOrder{ get; set; }
}
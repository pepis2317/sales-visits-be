using MediatR;

namespace sales_visits_be.Models.Locations;

public class DeleteVisitPlanRequest:IRequest<LocationResponse>
{
    public Guid SalesId { get; set; }
    public DateTime Date { get; set; }
    public int VisitOrder{ get; set; }
}
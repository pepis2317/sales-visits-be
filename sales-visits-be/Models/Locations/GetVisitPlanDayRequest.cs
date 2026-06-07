using MediatR;

namespace sales_visits_be.Models.Locations;

public class GetVisitPlanDayRequest:IRequest<GetVisitPlanDayResponse>
{
    public Guid SalesId { get; set; }
    public DateTime Date { get; set; }
}
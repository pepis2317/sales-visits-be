using MediatR;

namespace sales_visits_be.Models.Locations;

public class DailyVisitPlanRequest:IRequest<DailyVisitPlanResponse>
{
    public Guid SalesId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
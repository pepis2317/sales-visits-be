using MediatR;

namespace sales_visits_be.Models.Locations;

public class GetMonthlyVisitedLocationsRequest:IRequest<GetMonthlyVisitedLocationsResponse>
{
    public Guid SalesId { get; set; }
}
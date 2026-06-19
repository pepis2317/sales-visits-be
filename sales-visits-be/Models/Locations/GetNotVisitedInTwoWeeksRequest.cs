using MediatR;
using sales_visits_be.Handlers.Locations;

namespace sales_visits_be.Models.Locations;

public class GetNotVisitedInTwoWeeksRequest:IRequest<GetNotVisitedInTwoWeeksResponse>
{
    public Guid SalesId { get; set; }
    public int Page { get; set; }
    public int ItemsPerPage { get; set; }
}
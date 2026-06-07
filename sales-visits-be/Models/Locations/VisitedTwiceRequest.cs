using MediatR;

namespace sales_visits_be.Models.Locations;

public class VisitedTwiceRequest:IRequest<VisitedTwiceResponse>
{
    public Guid SalesId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int Page { get; set; }
    public int ItemsPerPage { get; set; }
}
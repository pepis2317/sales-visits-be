using MediatR;

namespace sales_visits_be.Models.Locations;

public class AccuracyReportRequest:IRequest<AccuracyReportResponse>
{
    public Guid SalesId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}
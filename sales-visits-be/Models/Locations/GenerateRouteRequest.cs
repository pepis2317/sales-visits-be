using MediatR;

namespace sales_visits_be.Models.Locations;

public class GenerateRouteRequest:IRequest<RouteResult>
{
    public Guid SalesId { get; set; }
    public int MaxCustomers { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
using MediatR;

namespace sales_visits_be.Models.Locations;

public class GetLocationsRequest:IRequest<GetLocationsResponse>
{
    public string? Query { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
using MediatR;

namespace sales_visits_be.Models.Locations;

public class GetLocationsListRequest: IRequest<GetLocationsListResponse>
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
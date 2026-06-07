using MediatR;

namespace sales_visits_be.Models.Locations;

public class AddLocationWithAddressRequest:IRequest<LocationResponse>
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
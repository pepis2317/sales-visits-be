using MediatR;

namespace sales_visits_be.Models.Locations;

public class LocationsTableRequest:IRequest<LocationsTableResponse>
{
    public string? Query { get; set; }
    public int Page {get; set;}
    public int ItemsPerPage {get; set;}
}
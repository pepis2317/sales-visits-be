using MediatR;

namespace sales_visits_be.Models.Locations;

public class VisitLocationRequest:IRequest<LocationResponse>
{
    public Guid SalesId { get; set; }
    public Guid LocationId { get; set; }
    public Guid TypeId { get; set; }
    public bool Recenter { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
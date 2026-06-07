using MediatR;

namespace sales_visits_be.Models.Locations;

public class DeleteLocationRequest:IRequest<LocationResponse>
{
    public Guid Id { get; set; }
}
using MediatR;

namespace sales_visits_be.Models.Locations;

public class EditLocationRequest:IRequest<LocationResponse>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Note { get; set; }
    public int? Potential { get; set; }
}
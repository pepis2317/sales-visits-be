using MediatR;

namespace sales_visits_be.Models.Locations;

public class RepositionListRequest : IRequest<RepositionListResponse>
{
    public int Page { get; set; }
    public int ItemsPerPage { get; set; }
}
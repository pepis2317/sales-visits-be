using MediatR;

namespace sales_visits_be.Models.Items;

public class ImportItemsBppRequest:IRequest<ItemsResponse>
{
    public List<string> BlobNames { get; set; } = new();
}
using MediatR;

namespace sales_visits_be.Models.Items;

public class ImportItemsSmdRequest : IRequest<ItemsResponse>
{
    public List<string> BlobNames { get; set; } = new();
}

public class ItemDTO
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}
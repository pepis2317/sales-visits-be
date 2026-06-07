namespace sales_visits_be.Models.Items;

public class ImportItemsRequest
{
    public List<string> BlobNames { get; set; } = new();
    public Guid WarehouseId { get; set; }
}
using entities.Entities;

namespace sales_visits_be.Models.Items;

public class ExportItemsResponse
{
    public Stream Stream { get; set; }
    public string ContentType { get; set; }
    public string FileName { get; set; }
}
public class ExportItemDTO
{
    public string NewJis { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public Guid? BrandId { get; set; }
}

namespace sales_visits_be.Models.Locations;

public class RepositionListResponse
{
    public List<RepositionData> Requests { get; set; } = new();
    public int TotalData { get; set; }
}

public class RepositionData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sales { get; set; } = string.Empty;
    public string OldAddress { get; set; } = string.Empty;
    public string NewAddress { get; set; } = string.Empty;
}
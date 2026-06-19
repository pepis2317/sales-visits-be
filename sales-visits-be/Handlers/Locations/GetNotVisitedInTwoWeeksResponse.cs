namespace sales_visits_be.Handlers.Locations;

public class GetNotVisitedInTwoWeeksResponse
{
    public List<NotVisitedData> Locations { get; set; } = new();
    public int TotalData { get; set; }
}
public class NotVisitedData
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime? LastVisitedAt { get; set; } 
}
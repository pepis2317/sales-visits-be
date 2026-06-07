namespace sales_visits_be.Models.Locations;

public class VisitedTwiceResponse
{
    public List<VisitedTwiceData> Locations { get; set; } = new();
    public int TotalData { get; set; }
    
}
public class VisitedTwiceData
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime? OldestVisit { get; set; }
    public DateTime? NewestVisit { get; set; }
}
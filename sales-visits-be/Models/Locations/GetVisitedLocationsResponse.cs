namespace sales_visits_be.Models.Locations;

public class GetVisitedLocationsResponse
{
    public List<VisitedListData> VisitedData { get; set; }
    public int TotalData { get; set; }
}
public class VisitedListData
{
    public string CustomerName { get; set; }
    public string Sales { get; set; }
    public string Note { get; set; }
    public DateTime VisitedAt { get; set; }
}
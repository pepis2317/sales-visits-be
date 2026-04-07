namespace sales_visits_be.Models.Locations;

public class GetLocationsListResponse
{
    public List<LocationData> Locations { get; set; } = new();
}
public class LocationData
{
    public string Name { get; set; }
    public string Address { get; set; }
    public DateTime? LastVisitedAt { get; set; }
    public double ApproximateDistance { get; set; }
}
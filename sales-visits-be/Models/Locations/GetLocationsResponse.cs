namespace sales_visits_be.Models.Locations;

public class GetLocationsResponse
{
    public List<LocationDropdown> Locations { get; set; } = new();
}
public class LocationDropdown
{
    public string Label { get; set; }
    public string Value { get; set; }
    public double ApproximateDistance { get; set; }
}
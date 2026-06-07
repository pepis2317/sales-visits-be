namespace sales_visits_be.Models.Locations;

public class GetVisitPlanLocationsResponse
{
    public List<Dropdown> Locations { get; set; } = new();
    public Dropdown InitialValue { get; set; } = new();
}
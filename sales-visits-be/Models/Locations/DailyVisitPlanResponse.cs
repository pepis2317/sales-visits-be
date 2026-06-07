namespace sales_visits_be.Models.Locations;

public class DailyVisitPlanResponse
{
    public List<LocationData> Locations { get; set; } = new();
    public List<string> Visited{ get; set; } = new();
}
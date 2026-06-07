namespace sales_visits_be.Models.Locations;

public class GetMonthlyVisitedLocationsResponse
{
    public List<MonthlyVisitedData> MonthlyLocations { get; set; }
}

public class MonthlyVisitedData
{
    public int Week { get; set; }
    public List<WeeklyVisitedData> WeeklyLocations { get; set; }
}
public class WeeklyVisitedData
{
    public int Day { get; set; }
    public List<VisitedData> Locations { get; set; }
}
public class VisitedData
{
    public string Name { get; set; }
    public string Address { get; set; }
    public DateTime VisitedAt { get; set; }
    public string Note { get; set; }
}
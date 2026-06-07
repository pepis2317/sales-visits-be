namespace sales_visits_be.Models.Locations;

public class AccuracyReportResponse
{
    public List<WeeklyReport> WeeklyReports { get; set; } = new();
}
public class WeeklyReport
{
    public int Week { get; set; }
    public List<AccuracyReport> Reports { get; set; } = new();
}
public class AccuracyReport
{
    public DateTime Date { get; set; }
    public List<string> VisitedLocations { get; set; } = new();
    public List<string>? PlannedLocations { get; set; } = new();
    public double? Accuracy { get; set; }
}
namespace sales_visits_be.Models;

public class RouteResult
{
    public List<CustomerPriorityScore> OrderedStops { get; set; }
    public long TotalDistanceMeters { get; set; }
    public bool SolutionFound { get; set; }
}
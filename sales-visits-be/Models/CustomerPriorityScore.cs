namespace sales_visits_be.Models;

public class CustomerPriorityScore
{
    public Guid CustomerLocationId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double PriorityScore { get; set; }
    
    public double RecencyScore { get; set; }
    public double FrequencyScore { get; set; }
    public double ConsistencyScore { get; set; }
    public DateTime LastVisitedAt { get; set; }
}
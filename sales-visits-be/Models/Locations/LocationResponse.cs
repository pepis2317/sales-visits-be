namespace sales_visits_be.Models.Locations;

public class LocationResponse
{
    public Guid Id { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}
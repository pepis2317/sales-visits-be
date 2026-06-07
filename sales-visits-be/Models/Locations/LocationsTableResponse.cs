namespace sales_visits_be.Models.Locations;

public class LocationsTableResponse
{
    public List<LocationsTableData> Locations { get; set; } = new();
    public int TotalData { get; set; }
}
public class LocationsTableData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Potential { get; set; }
    public string Note { get; set; } = string.Empty;

}
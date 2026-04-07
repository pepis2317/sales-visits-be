using entities.Entities;

namespace sales_visits_be.Models;

public class TerritoryResult
{
    public Guid SalesId { get; set; }
    public List<CustomerLocation> Customers { get; set; }
    public double CentroidLatitude { get; set; }
    public double CentroidLongitude { get; set; }
    public double RadiusMeters { get; set; }
}
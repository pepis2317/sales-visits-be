using entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using sales_visits_be.Models;

namespace sales_visits_be.Service;

public class TerritoryService
{
    private readonly ApplicationDbContext _db;
    private const double ExpansionFactor = 1.25;

    public TerritoryService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<TerritoryResult> GetTerritoryAsync(Guid salesId, CancellationToken cancellationToken)
    {
        var visitedCustomers = await _db.SalesVisits
            .Where(q => q.SalesId == salesId)
            .Select(q => q.CustomerLocation)
            .Distinct().ToListAsync(cancellationToken);
        if (!visitedCustomers.Any())
        {
            return new TerritoryResult
            {
                SalesId = salesId,
                Customers = [],
                RadiusMeters = 0
            };
        }

        double centroidLat = visitedCustomers.Average(q => q.Location.Y);
        double centroidLon = visitedCustomers.Average(q => q.Location.X);

        var distances = visitedCustomers
            .Select(q => HaversineMeters(centroidLat, centroidLon, q.Location.Y, q.Location.X)).ToList();
        
        double mean = distances.Average();
        double stdDev = Math.Sqrt(distances.Average(q => Math.Pow(q - mean, 2)));
        double radiusMeters = (mean + 2 * stdDev) * ExpansionFactor;

        var centroidPoint = new Point(centroidLon, centroidLat) { SRID = 4326 };
        var territory = await _db.CustomerLocations
            .Where(q => q.Location.IsWithinDistance(centroidPoint, radiusMeters))
            .ToListAsync(cancellationToken);

        return new TerritoryResult
        {
            SalesId = salesId,
            Customers = territory,
            CentroidLatitude = centroidLat,
            CentroidLongitude = centroidLon,
            RadiusMeters = radiusMeters
        };
    }

    private static double HaversineMeters(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6_371_000;

        double dLat = ToRad(lat2 - lat1);
        double dLon = ToRad(lon2 - lon1);

        double a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double ToRad(double deg) => deg * Math.PI / 180;
}
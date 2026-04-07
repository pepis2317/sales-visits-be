using sales_visits_be.Models;

namespace sales_visits_be.Service;

public class DistanceMatrixService
{
    private const double EarthRadiusMeters = 6_371_000;

    public DistanceMatrixResult Build(List<CustomerPriorityScore> customers, double? depotLat = null , double? depotLon = null)
    {
        if(depotLat.HasValue && depotLon.HasValue)
        {
            var depot = new CustomerPriorityScore
            {
                CustomerName = "_depot",
                Latitude = depotLat.Value,
                Longitude = depotLon.Value
            };
            customers = customers.Prepend(depot).ToList();
        }
        int n = customers.Count;
        var matrix = new long[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (i == j)
                {
                    matrix[i, j] = 0;
                    continue;
                }

                matrix[i, j] = HaversineMeters(
                    customers[i].Latitude, customers[i].Longitude,
                    customers[j].Latitude, customers[j].Longitude
                );
            }
        }

        return new DistanceMatrixResult
        {
            Customers = customers,
            Matrix = matrix
        };
    }

    private static long HaversineMeters(double lat1, double long1, double lat2, double long2)
    {
        double dLat = ToRad(lat2 - lat1);
        double dLon = ToRad(long2 - long1);

        double a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return (long)(EarthRadiusMeters * c);
    }

    private static double ToRad(double degrees) => degrees * Math.PI / 180.0;
}
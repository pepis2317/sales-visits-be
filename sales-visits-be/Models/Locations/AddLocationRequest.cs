using System.Text.Json.Serialization;
using MediatR;
using NetTopologySuite.Geometries;

namespace sales_visits_be.Models.Locations;

public class AddLocationRequest:IRequest<LocationResponse>
{
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
public class GoogleGeocodingResponse
{
    [JsonPropertyName("results")]
    public List<GeocodingResult> Results { get; set; }
}

public class GeocodingResult
{
    [JsonPropertyName("formatted_address")]
    public string FormattedAddress { get; set; }
    [JsonPropertyName("geometry")]
    public Geometry Geometry { get; set; }
}
public class Geometry
{
    [JsonPropertyName("location")]
    public Location Location { get; set; }
}

public class Location
{
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lng")]
    public double Lng { get; set; }
}
using entities;
using entities.Entities;
using MediatR;
using NetTopologySuite.Geometries;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class AddLocationHandler:IRequestHandler<AddLocationRequest, LocationResponse>
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    public AddLocationHandler(ApplicationDbContext db, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _db = db;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }
    public async Task<LocationResponse> Handle(AddLocationRequest request, CancellationToken cancellationToken)
    {
        var locationId = Guid.NewGuid();
        var incomingPoint = new Point(request.Longitude, request.Latitude) { SRID = 4326 };
        var apiKey = _configuration["GoogleMaps:ApiKey"];
        var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={request.Latitude},{request.Longitude}&key={apiKey}";

        using var http = _httpClientFactory.CreateClient();
        var response = await http.GetFromJsonAsync<GoogleGeocodingResponse>(url, cancellationToken);

        var address = response?.Results?.FirstOrDefault()?.FormattedAddress ?? "Unknown address";
        var location = new CustomerLocation
        {
            Id =  locationId,
            Name = request.Name.ToUpper(),
            Location = incomingPoint,
            Address = address.ToUpper(),
            LastVisitedAt = DateTime.UtcNow
        };
        _db.CustomerLocations.Add(location);
        await _db.SaveChangesAsync(cancellationToken);
        return new LocationResponse
        {
            IsSuccess = true,
            Message = $"{location.Name} has been added ({address})"
        };
    }
}
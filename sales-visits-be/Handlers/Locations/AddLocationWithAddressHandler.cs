using entities;
using entities.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class AddLocationWithAddressHandler : IRequestHandler<AddLocationWithAddressRequest, LocationResponse>
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public AddLocationWithAddressHandler(ApplicationDbContext db, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _db = db;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<LocationResponse> Handle(AddLocationWithAddressRequest request, CancellationToken cancellationToken)
    {
        var check = await _db.CustomerLocations
            .FirstOrDefaultAsync(q => q.Name.Trim().ToUpper() == request.Name.Trim().ToUpper(), cancellationToken);
        if (check != null)
        {
            return new LocationResponse
            {
                IsSuccess = false,
                Message = $"{request.Name} already exists"
            };
        }
        var locationId = Guid.NewGuid();
        var apiKey = _configuration["GoogleMaps:ApiKey"];
        var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={request.Address}&key={apiKey}";

        using var http = _httpClientFactory.CreateClient();
        var response = await http.GetFromJsonAsync<GoogleGeocodingResponse>(url, cancellationToken);

        var result = response?.Results.FirstOrDefault();
        var lat = result?.Geometry?.Location?.Lat;
        var lng = result?.Geometry?.Location?.Lng;
        var address = result?.FormattedAddress;
        if (lat == null || lng == null || address == null)
        {
            return new LocationResponse
            {
                IsSuccess = false,
                Message = "Failed to get location address"
            };
        }

        var location = new CustomerLocation
        {
            Id = locationId,
            Name = request.Name.ToUpper(),
            Location = new Point((double)lng, (double)lat) { SRID = 4326 },
            Address = address.ToUpper()
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
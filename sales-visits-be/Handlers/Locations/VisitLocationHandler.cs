using entities;
using entities.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class VisitLocationHandler:IRequestHandler<VisitLocationRequest, LocationResponse>
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    public VisitLocationHandler(ApplicationDbContext db, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }
    
    public async Task<LocationResponse> Handle(VisitLocationRequest request, CancellationToken cancellationToken)
    {
        var location = await _db.CustomerLocations.FirstOrDefaultAsync(q => q.Id == request.LocationId, cancellationToken);
        if(location == null || location.Location == null)
        {
            return new LocationResponse
            {
                IsSuccess = false,
                Message = "Location not found"
            };
        }
        location.LastVisitedAt = DateTime.UtcNow;
        var incomingPoint = new Point(request.Longitude, request.Latitude) { SRID = 4326 };
        if(request.Recenter)
        {
            var apiKey = _configuration["GoogleMaps:ApiKey"];
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={request.Latitude},{request.Longitude}&key={apiKey}";

            using var http = _httpClientFactory.CreateClient();
            var response = await http.GetFromJsonAsync<GoogleGeocodingResponse>(url, cancellationToken);

            var address = response?.Results?.FirstOrDefault()?.FormattedAddress ?? "Unknown address";
            _db.RepositionRequests.Add(new RepositionRequest
            {
                SalesId = request.SalesId,
                CustomerLocationId =  location.Id,
                OldPosition = location.Location,
                NewPosition = incomingPoint,
                Address = address.ToUpper()
            });
        }

        var visit = new SalesVisit
        {
            CustomerLocationId = location.Id,
            SalesId = request.SalesId,
            CreatedAt =  DateTime.UtcNow,
            VisitTypeId =  request.TypeId,
        };
        
        _db.SalesVisits.Add(visit);
        await _db.SaveChangesAsync(cancellationToken);
        return new LocationResponse
        {
            IsSuccess = true,
            Message = $"{location.Name} has been visited"
        };
    }
}
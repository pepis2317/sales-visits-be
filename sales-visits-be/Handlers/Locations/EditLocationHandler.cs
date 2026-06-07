using entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class EditLocationHandler:IRequestHandler<EditLocationRequest, LocationResponse>
{
    private readonly ApplicationDbContext _db;
    public EditLocationHandler(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<LocationResponse> Handle(EditLocationRequest request, CancellationToken cancellationToken)
    {
        var location = await _db.CustomerLocations.FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);
        if(location == null)
        {
            return new LocationResponse
            {
                IsSuccess = false,
                Message = "Location not found"
            };
        }

        location.Note = !string.IsNullOrEmpty(request.Note) ? request.Note : location.Note;
        location.Name = !string.IsNullOrEmpty(request.Name) ? request.Name: location.Name;
        location.Address= !string.IsNullOrEmpty(request.Address) ? request.Address : location.Address;
        location.Potential = request.Potential ?? location.Potential;
        location.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return new LocationResponse
        {
            IsSuccess = true,
            Message = "Location updated"
        };
    }
}
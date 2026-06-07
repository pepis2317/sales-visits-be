using entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class DeleteLocationHandler:IRequestHandler<DeleteLocationRequest, LocationResponse>
{
    private readonly ApplicationDbContext _db;
    public DeleteLocationHandler(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<LocationResponse> Handle(DeleteLocationRequest request, CancellationToken cancellationToken)
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
        var visits = await _db.SalesVisits.Where(q => q.CustomerLocationId == request.Id)
            .ToListAsync(cancellationToken);
        _db.RemoveRange(visits);
        _db.Remove(location);
        await _db.SaveChangesAsync(cancellationToken);
        return new LocationResponse
        {
            IsSuccess = true,
            Message = "Successfully deleted location"
        };
    }
}
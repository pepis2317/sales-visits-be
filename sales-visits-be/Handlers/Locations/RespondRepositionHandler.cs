using entities;
using entities.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Handlers.Locations;

public class RespondRepositionHandler : IRequestHandler<RespondRepositionRequest, LocationResponse>
{
    private readonly ApplicationDbContext _db;

    public RespondRepositionHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<LocationResponse> Handle(RespondRepositionRequest request, CancellationToken cancellationToken)
    {
        var reposition = await _db.RepositionRequests
            .FirstOrDefaultAsync(q => q.Id == request.RepositionRequestId, cancellationToken);
        if (reposition == null)
        {
            return new LocationResponse
            {
                IsSuccess = false,
                Message = "Request not found"
            };
        }

        if (request.IsApproved)
        {
            var location = await _db.CustomerLocations
                    .FirstOrDefaultAsync(q => q.Id == reposition.CustomerLocationId, cancellationToken);
            location.Location = reposition.NewPosition;
            location.Address = reposition.Address;
            
            reposition.AcceptedAt = DateTime.UtcNow;
        }
        else
        {
            reposition.DeclinedAt = DateTime.UtcNow;
        }
        
        await _db.SaveChangesAsync(cancellationToken);
        return new LocationResponse
        {
            IsSuccess = true,
            Message = request.IsApproved ? "Request approved" : "Request declined",
        };

        throw new NotImplementedException();
    }
}
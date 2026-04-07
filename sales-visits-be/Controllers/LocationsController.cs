using MediatR;
using Microsoft.AspNetCore.Mvc;
using sales_visits_be.Models;
using sales_visits_be.Models.Locations;

namespace sales_visits_be.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LocationsController:ControllerBase
{
    private readonly IMediator _mediator;
    public LocationsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("get-locations-list")]
    public async Task<ActionResult<GetLocationsListResponse>> GetList ([FromQuery]GetLocationsListRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
    [HttpGet("get-locations-filter")]
    public async Task<ActionResult<GetLocationsResponse>> GetFilter([FromQuery] GetLocationsRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
    [HttpPost("visit-location")]
    public async Task<ActionResult<LocationResponse>> VisitLocation([FromBody] VisitLocationRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
    [HttpPost("add-location")]
    public async Task<ActionResult<LocationResponse>> AddLocation([FromBody] AddLocationRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
    [HttpGet("generate-route")]
    public async Task<ActionResult<RouteResult>> GenerateRoute([FromQuery] GenerateRouteRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
    [HttpPut("respond-reposition-request")]
    public async Task<ActionResult<LocationResponse>> RespondRepositionRequest([FromBody] RespondRepositionRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
}
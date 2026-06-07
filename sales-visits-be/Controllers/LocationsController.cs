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
    
    [HttpPost("add-location-address")]
    public async Task<ActionResult<LocationResponse>> AddLocationAddress([FromBody] AddLocationWithAddressRequest request)
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
    
    [HttpGet("get-visited-monthly")]
    public async Task<ActionResult<GetMonthlyVisitedLocationsResponse>> GetMonthlyVisitedLocations([FromQuery] GetMonthlyVisitedLocationsRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
 
    [HttpGet("get-visited")]
    public async Task<ActionResult<GetVisitedLocationsResponse>> GetVisitedLocations([FromQuery] GetVisitedLocationsRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
    [HttpGet("get-visit-plan-locations")]
    public async Task<ActionResult<GetVisitPlanLocationsResponse>> GetVisitPlanLocations([FromQuery] GetVisitPlanLocationsRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
    [HttpPut("visit-plan")]
    public async Task<ActionResult<LocationResponse>> CreateVisitPlans([FromBody] CreateVisitPlanRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
    [HttpGet("get-visit-plan-day")]
    public async Task<ActionResult<GetVisitPlanDayResponse>> GetVisitPlans([FromQuery] GetVisitPlanDayRequest dayRequest)
    {
        var response = await _mediator.Send(dayRequest);
        return Ok(response);
    }
    
    [HttpDelete("delete-visit-plan")]
    public async Task<ActionResult<LocationResponse>> DeleteVisitPlan([FromQuery] DeleteVisitPlanRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
    [HttpGet("get-accuracy-report")]
    public async Task<ActionResult<AccuracyReportResponse>> GetAccuracyReport([FromQuery] AccuracyReportRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
    [HttpGet("get-daily-visit-plan")]
    public async Task<ActionResult<DailyVisitPlanResponse>> GetdailyVisitPlan([FromQuery] DailyVisitPlanRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
    [HttpGet("get-visited-twice")]
    public async Task<ActionResult<VisitedTwiceResponse>> GetVisitedTwice([FromQuery] VisitedTwiceRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
    [HttpGet("get-not-visited-twice")]
    public async Task<ActionResult<VisitedTwiceResponse>> GetNotVisitedTwice([FromQuery] NotVisitedTwiceRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
    [HttpGet("get-locations-table")]
    public async Task<ActionResult<LocationsTableResponse>> GetLocationsTable([FromQuery] LocationsTableRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
    [HttpPut("edit-location")]
    public async Task<ActionResult<LocationResponse>> EditLocation([FromBody] EditLocationRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
    [HttpDelete("delete-location")]
    public async Task<ActionResult<LocationResponse>> DeleteLocation([FromQuery] DeleteLocationRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
    [HttpGet("get-reposition-requests")]
    public async Task<ActionResult<RepositionListResponse>> GetRepositionRequests([FromQuery] RepositionListRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
    [HttpPost("manual-visit")]
    public async Task<ActionResult<LocationResponse>> ManualVisit([FromBody] ManualVisitRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
        
}
using MediatR;
using Microsoft.AspNetCore.Mvc;
using sales_visits_be.Models.Sales;
using sales_visits_be.Models.VisitTypes;

namespace sales_visits_be.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SalesController:ControllerBase
{
    private readonly IMediator _mediator;
    public  SalesController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet("get-sales")]
    public async Task<ActionResult<GetSalesResponse>> GetSales([FromQuery] GetSalesRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    [HttpGet("get-visit-types")]
    public async Task<ActionResult<GetVisitTypesResponse>> GetSales([FromQuery] GetVisitTypesRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
}

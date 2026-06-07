using MediatR;
using Microsoft.AspNetCore.Mvc;
using sales_visits_be.Enums;
using sales_visits_be.Models.Items;

namespace sales_visits_be.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ItemsController:ControllerBase
{
    private readonly IMediator _mediator;
    public ItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("import-items")]
    public async Task<ActionResult<ItemsResponse>> ImportItems([FromBody] ImportItemsRequest request)
    {
        if(request.WarehouseId == WarehouseEnums.Samarinda)
        {
            var resultSmd = await _mediator.Send(new ImportItemsSmdRequest{BlobNames =  request.BlobNames});
            return Ok(resultSmd);
        }
        var result = await _mediator.Send(new ImportItemsBppRequest{BlobNames = request.BlobNames});
        return Ok(result);
    }
    
    [HttpGet("export-items")]
    public async Task<ActionResult> ExportItems([FromQuery] ExportItemsRequest request)
    {
        var result = await _mediator.Send(request);
        return File(result.Stream, result.ContentType, result.FileName);
    }
    [HttpGet("get-brands")]
    public async Task<ActionResult<GetBrandsResponse>> GetBrands([FromQuery] GetBrandsRequest request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }
    [HttpGet("get-warehouses")]
    public async Task<ActionResult<GetWarehousesResponse>> GetWarehouses([FromQuery] GetWarehousesRequest request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }
}

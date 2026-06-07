using MediatR;
using Microsoft.AspNetCore.Mvc;
using sales_visits_be.Models.Blobs;

namespace sales_visits_be.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BlobController : ControllerBase
{
    private readonly IMediator _mediator;
    public BlobController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("upload-file")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10_485_760)]
    public async Task<ActionResult<FileResponse>> UploadFileToBackend([FromForm] UploadFileRequest request)
    {
        var response = await _mediator.Send(request);
        if(!response.IsSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
    
    [HttpGet("files/{*fileName}")]
    public async Task<IActionResult> GetFile(string fileName)
    {
        var response = await _mediator.Send(new GetFileRequest{FileName = fileName});
        if(!response.IsSuccess || response.Stream == null || response.ContentType == null)
        {
            return BadRequest(response.Message);
        }
        return File(response.Stream, response.ContentType, response.FileName);
    }
    
    [HttpDelete("delete-file/{*fileName}")]
    public async Task<ActionResult<FileResponse>> DeleteFile(string fileName)
    {
        var response = await _mediator.Send(new DeleteFileRequest{FileName = fileName});
        if(!response.IsSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
}

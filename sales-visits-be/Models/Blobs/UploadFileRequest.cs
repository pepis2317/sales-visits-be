using MediatR;

namespace sales_visits_be.Models.Blobs;

public class UploadFileRequest:IRequest<FileResponse>
{
    public IFormFile File { get; set; }
    public string? CustomFileName { get; set; }
}

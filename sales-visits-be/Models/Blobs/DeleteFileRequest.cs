using MediatR;

namespace sales_visits_be.Models.Blobs;

public class DeleteFileRequest:IRequest<FileResponse>
{
    public string FileName { get; set; } = string.Empty;
}

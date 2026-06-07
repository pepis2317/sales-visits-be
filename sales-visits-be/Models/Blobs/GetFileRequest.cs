using MediatR;

namespace sales_visits_be.Models.Blobs;

public class GetFileRequest:IRequest<FileResponse>
{
    public string FileName { get; set; }
}

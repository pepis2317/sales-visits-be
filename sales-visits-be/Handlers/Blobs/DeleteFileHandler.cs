using MediatR;
using sales_visits_be.Models.Blobs;
using sales_visits_be.Service;

namespace sales_visits_be.Handlers.Blobs;

public class DeleteFileHandler:IRequestHandler<DeleteFileRequest, FileResponse>
{
    private readonly BlobService _service;
    public DeleteFileHandler(BlobService service)
    {
        _service = service;
    }
    public async Task<FileResponse> Handle(DeleteFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _service.DeleteAsync(request.FileName);
        }
        catch(Exception e)
        {
            return new FileResponse
            {
                Message = e.InnerException.ToString(),
                IsSuccess = false
            };
        }

        return new FileResponse
        {
            Message = $"File {request.FileName} has been successfully deleted" ,
            IsSuccess = true
        };
    }
}
using MediatR;
using sales_visits_be.Models.Blobs;
using sales_visits_be.Service;

namespace sales_visits_be.Handlers.Blobs;

public class GetFileHandler:IRequestHandler<GetFileRequest, FileResponse>
{
    private readonly BlobService _service;

    public GetFileHandler(BlobService service)
    {
        _service = service;
    }
    public async Task<FileResponse> Handle(GetFileRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.FileName))
        {
            return new FileResponse
            {
                Message = "Filename required",
                IsSuccess = false
            };
        }

        var (stream, contentType) = await _service.GetObjectAsync(request.FileName);
        if (stream == null || contentType == null)
        {
            return new FileResponse
            {
                Message = "Unable to get object from storage",
                IsSuccess = false
            };
        }

        return new FileResponse
        {
            Message = "Success",
            IsSuccess = true,
            Stream = stream,
            ContentType = contentType,
            FileName = request.FileName
        };
    }
}
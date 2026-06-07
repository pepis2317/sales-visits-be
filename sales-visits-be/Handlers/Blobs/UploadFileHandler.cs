using MediatR;
using sales_visits_be.Models.Blobs;
using sales_visits_be.Service;

namespace sales_visits_be.Handlers.Blobs;

public class UploadFileHandler:IRequestHandler<UploadFileRequest, FileResponse>
{
    private readonly BlobService _service;
    public UploadFileHandler(BlobService service)
    {
        _service = service;
    }
    public async Task<FileResponse> Handle(UploadFileRequest request, CancellationToken cancellationToken)
    {
        if(request.File.Length == 0)
        {
            return new FileResponse
            {
                Message = "No file uploaded or file is empty",
                IsSuccess = false
            };
        }

        string finalFileName = request.CustomFileName ?? request.File.FileName;
        finalFileName = Path.GetFileName(finalFileName);
        
        try
        {
            await _service.UploadAsync(request.File.OpenReadStream(), finalFileName, request.File.ContentType);
            return new FileResponse
            {
                Message = $"File '{finalFileName}' uploaded successfully to backend and then to storage.",
                IsSuccess = true
            };
        }
        catch (ArgumentException ex)
        {
            return new FileResponse
            {
                Message = ex.Message,
                IsSuccess =  false
            };
        }
        catch (Exception ex)
        {
            return new FileResponse
            {
                Message = $"Internal server error during file upload: {ex.Message}",
                IsSuccess = false
            };
        }
    }
}
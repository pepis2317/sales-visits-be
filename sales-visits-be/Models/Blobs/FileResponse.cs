namespace sales_visits_be.Models.Blobs;

public class FileResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Stream? Stream { get; set; }
    public string? ContentType { get; set; }
    public string? FileName { get; set; }
}
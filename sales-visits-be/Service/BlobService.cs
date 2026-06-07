using System.Reflection.Metadata;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using entities;
using Microsoft.EntityFrameworkCore;
using Blob = entities.Entities.Blob;

namespace sales_visits_be.Service;

public class BlobService
{
    private readonly BlobContainerClient _container;
    private readonly ApplicationDbContext _db;
    public BlobService(IConfiguration config, ApplicationDbContext db)
    {
        _db = db;
        var client = new BlobServiceClient(config["AzureStorage:ConnectionString"]);
        _container = client.GetBlobContainerClient(config["AzureStorage:ContainerName"]);
    }
    
    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType)
    {
        string finalName = await GetUniqueFileName(fileName);
        
        var blob = _container.GetBlobClient(finalName);
        await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType });
        
        _db.Blobs.Add(new Blob
        {
            Filename =  fileName,
            BlobName =  blob.Name,
            ContentType = contentType,
            SizeBytes = stream.Length,
        });
        await _db.SaveChangesAsync();
        return blob.Name;
    }
    
    public async Task<(Stream? stream, string? contentType)> GetObjectAsync(string fileName)
    {
        try
        {
            var blob  = _container.GetBlobClient(fileName);
            if(!await blob.ExistsAsync())
            {
                return (null, null);
            }
            
            var properties = await blob.GetPropertiesAsync();
            var contentType = properties.Value.ContentType;

            var stream = new MemoryStream();
            await blob.DownloadToAsync(stream);
            stream.Position = 0;
            return (stream, contentType);
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            return (null, null);
        }
    }
    
    public async Task DeleteAsync(string blobName)
    {
        await _db.Blobs.Where(q => q.BlobName == blobName).ExecuteDeleteAsync();
        await _container.DeleteBlobIfExistsAsync(blobName);
    }
    
    private async Task<string> GetUniqueFileName(string fileName)
    {
        if (!await _container.GetBlobClient(fileName).ExistsAsync())
            return fileName;

        string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
        string ext = Path.GetExtension(fileName);
        int counter = 1;

        string candidate;
        do
        {
            candidate = $"{nameWithoutExt}({counter}){ext}";
            counter++;
        } 
        while (await _container.GetBlobClient(candidate).ExistsAsync());

        return candidate;
    }
}
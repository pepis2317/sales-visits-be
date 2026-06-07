using System;
using System.Collections.Generic;

namespace entities.Entities;

public partial class Blob
{
    public Guid Id { get; set; }

    public string Filename { get; set; } = null!;

    public string BlobName { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public long? SizeBytes { get; set; }

    public DateTime UploadedAt { get; set; }
}

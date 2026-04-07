using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace entities.Entities;

public partial class RepositionRequest
{
    public Guid Id { get; set; }

    public Guid SalesId { get; set; }

    public Guid CustomerLocationId { get; set; }

    public Point? OldPosition { get; set; }

    public Point? NewPosition { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? AcceptedAt { get; set; }

    public DateTime? DeclinedAt { get; set; }

    public string? Address { get; set; }

    public virtual CustomerLocation CustomerLocation { get; set; } = null!;

    public virtual Sale Sales { get; set; } = null!;
}

using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace entities.Entities;

public partial class CustomerLocation
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Point? Location { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastVisitedAt { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<RepositionRequest> RepositionRequests { get; set; } = new List<RepositionRequest>();

    public virtual ICollection<SalesVisit> SalesVisits { get; set; } = new List<SalesVisit>();
}

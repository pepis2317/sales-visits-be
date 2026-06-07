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

    public int? Potential { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<CustomerTarget> CustomerTargets { get; set; } = new List<CustomerTarget>();

    public virtual ICollection<RepositionRequest> RepositionRequests { get; set; } = new List<RepositionRequest>();

    public virtual ICollection<SalesVisit> SalesVisits { get; set; } = new List<SalesVisit>();

    public virtual ICollection<VisitPlan> VisitPlans { get; set; } = new List<VisitPlan>();
}

using System;
using System.Collections.Generic;

namespace entities.Entities;

public partial class Sale
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<RepositionRequest> RepositionRequests { get; set; } = new List<RepositionRequest>();

    public virtual ICollection<SalesVisit> SalesVisits { get; set; } = new List<SalesVisit>();
}

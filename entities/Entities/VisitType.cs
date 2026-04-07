using System;
using System.Collections.Generic;

namespace entities.Entities;

public partial class VisitType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<SalesVisit> SalesVisits { get; set; } = new List<SalesVisit>();
}

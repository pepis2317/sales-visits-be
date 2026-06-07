using System;
using System.Collections.Generic;

namespace entities.Entities;

public partial class VisitPlan
{
    public Guid Id { get; set; }

    public Guid CustomerLocationId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime Date { get; set; }

    public Guid SalesId { get; set; }

    public int VisitOrder { get; set; }

    public virtual CustomerLocation CustomerLocation { get; set; } = null!;

    public virtual Sale Sales { get; set; } = null!;
}

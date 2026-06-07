using System;
using System.Collections.Generic;

namespace entities.Entities;

public partial class SalesVisit
{
    public Guid Id { get; set; }

    public Guid CustomerLocationId { get; set; }

    public Guid SalesId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid VisitTypeId { get; set; }

    public string? Note { get; set; }

    public virtual CustomerLocation CustomerLocation { get; set; } = null!;

    public virtual Sale Sales { get; set; } = null!;

    public virtual VisitType VisitType { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace entities.Entities;

public partial class CustomerTarget
{
    public Guid Id { get; set; }

    public Guid CustomerLocationId { get; set; }

    public string TargetName { get; set; } = null!;

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual CustomerLocation CustomerLocation { get; set; } = null!;
}

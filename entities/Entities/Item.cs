using System;
using System.Collections.Generic;

namespace entities.Entities;

public partial class Item
{
    public Guid Id { get; set; }

    public Guid? BrandId { get; set; }

    public Guid? TypeId { get; set; }

    public Guid? UnitId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? ShortName { get; set; }

    public Guid? WarehouseId { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual ItemType? Type { get; set; }

    public virtual ItemUnit? Unit { get; set; }

    public virtual Warehouse? Warehouse { get; set; }
}

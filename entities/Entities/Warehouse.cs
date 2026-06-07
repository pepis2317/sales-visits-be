using System;
using System.Collections.Generic;

namespace entities.Entities;

public partial class Warehouse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}

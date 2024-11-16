using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class OrderActivityType
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<OrderActivity> OrderActivities { get; set; } = new List<OrderActivity>();
}

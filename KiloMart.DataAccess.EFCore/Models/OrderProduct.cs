using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class OrderProduct
{
    public int Id { get; set; }

    public long Order { get; set; }

    public int Product { get; set; }

    public double Quantity { get; set; }

    public virtual Order OrderNavigation { get; set; } = null!;

    public virtual Product ProductNavigation { get; set; } = null!;
}

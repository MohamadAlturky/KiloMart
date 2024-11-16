using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class ProductDiscount
{
    public long Id { get; set; }

    public int Product { get; set; }

    public int DiscountCode { get; set; }

    public DateTime AssignedDate { get; set; }

    public bool IsActive { get; set; }

    public virtual DiscountCode DiscountCodeNavigation { get; set; } = null!;

    public virtual Product ProductNavigation { get; set; } = null!;
}

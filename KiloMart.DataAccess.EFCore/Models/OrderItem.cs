using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class OrderItem
{
    public long Id { get; set; }

    public long Order { get; set; }

    public int ProductOffer { get; set; }

    public decimal UnitPrice { get; set; }

    public double Quantity { get; set; }

    public virtual Order OrderNavigation { get; set; } = null!;

    public virtual ProductOffer ProductOfferNavigation { get; set; } = null!;

    public virtual ICollection<DiscountCode> DiscountCodes { get; set; } = new List<DiscountCode>();
}

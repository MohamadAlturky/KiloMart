using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class OrderDeliveryInformation
{
    public long Id { get; set; }

    public long Order { get; set; }

    public int Delivery { get; set; }

    public virtual Delivery DeliveryNavigation { get; set; } = null!;

    public virtual Order OrderNavigation { get; set; } = null!;
}

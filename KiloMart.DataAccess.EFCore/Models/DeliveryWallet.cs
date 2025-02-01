using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class DeliveryWallet
{
    public int Id { get; set; }

    public double Value { get; set; }

    public int Delivery { get; set; }

    public virtual Delivery DeliveryNavigation { get; set; } = null!;
}

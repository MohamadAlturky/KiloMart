using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class DeliveryActivity
{
    public long Id { get; set; }

    public DateTime Date { get; set; }

    public double Value { get; set; }

    public byte Type { get; set; }

    public int Delivery { get; set; }

    public virtual Delivery DeliveryNavigation { get; set; } = null!;

    public virtual DeliveryActivityType TypeNavigation { get; set; } = null!;
}

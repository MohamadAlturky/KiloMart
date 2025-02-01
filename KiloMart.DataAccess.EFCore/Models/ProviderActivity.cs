using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class ProviderActivity
{
    public long Id { get; set; }

    public DateTime Date { get; set; }

    public double Value { get; set; }

    public int Provider { get; set; }

    public byte Type { get; set; }

    public virtual Provider ProviderNavigation { get; set; } = null!;

    public virtual DeliveryActivityType TypeNavigation { get; set; } = null!;
}

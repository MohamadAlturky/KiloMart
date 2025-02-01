using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class OrderProviderInformation
{
    public long Id { get; set; }

    public long Order { get; set; }

    public int Provider { get; set; }

    public int Location { get; set; }

    public virtual Location LocationNavigation { get; set; } = null!;

    public virtual Order OrderNavigation { get; set; } = null!;

    public virtual Provider ProviderNavigation { get; set; } = null!;
}

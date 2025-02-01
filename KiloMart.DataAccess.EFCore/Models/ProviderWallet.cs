using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class ProviderWallet
{
    public int Id { get; set; }

    public double Value { get; set; }

    public int Provider { get; set; }

    public virtual Provider ProviderNavigation { get; set; } = null!;
}

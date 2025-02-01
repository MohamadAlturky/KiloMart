using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class SystemActivity
{
    public long Id { get; set; }

    public DateTime Date { get; set; }

    public double Value { get; set; }

    public long Order { get; set; }

    public virtual Order OrderNavigation { get; set; } = null!;
}

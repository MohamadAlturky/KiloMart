using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class OrderCustomerInformation
{
    public long Id { get; set; }

    public long Order { get; set; }

    public int Customer { get; set; }

    public int Location { get; set; }

    public virtual Customer CustomerNavigation { get; set; } = null!;

    public virtual Location LocationNavigation { get; set; } = null!;

    public virtual Order OrderNavigation { get; set; } = null!;
}

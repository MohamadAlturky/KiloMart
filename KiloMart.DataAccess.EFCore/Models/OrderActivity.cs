using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class OrderActivity
{
    public long Id { get; set; }

    public long Order { get; set; }

    public DateTime Date { get; set; }

    public byte OrderActivityType { get; set; }

    public int OperatedBy { get; set; }

    public virtual Party OperatedByNavigation { get; set; } = null!;

    public virtual OrderActivityType OrderActivityTypeNavigation { get; set; } = null!;

    public virtual Order OrderNavigation { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class SearchHistory
{
    public long Id { get; set; }

    public int Customer { get; set; }

    public string Term { get; set; } = null!;

    public virtual Party CustomerNavigation { get; set; } = null!;
}

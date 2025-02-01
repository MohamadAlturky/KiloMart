using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class FavoriteProduct
{
    public long Id { get; set; }

    public int Customer { get; set; }

    public int Product { get; set; }

    public virtual Customer CustomerNavigation { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class DiscountType
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<DiscountCode> DiscountCodes { get; set; } = new List<DiscountCode>();
}

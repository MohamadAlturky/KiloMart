using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class ProductLocalized
{
    public byte Language { get; set; }

    public int Product { get; set; }

    public string MeasurementUnit { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual Language LanguageNavigation { get; set; } = null!;

    public virtual Product ProductNavigation { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class ProductRequestDataLocalized
{
    public int ProductRequest { get; set; }

    public byte Language { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string MeasurementUnit { get; set; } = null!;

    public virtual Language LanguageNavigation { get; set; } = null!;

    public virtual ProductRequest ProductRequestNavigation { get; set; } = null!;
}

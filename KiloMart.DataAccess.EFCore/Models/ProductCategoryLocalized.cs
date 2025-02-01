using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class ProductCategoryLocalized
{
    public string Name { get; set; } = null!;

    public byte Language { get; set; }

    public int ProductCategory { get; set; }

    public virtual Language LanguageNavigation { get; set; } = null!;

    public virtual ProductCategory ProductCategoryNavigation { get; set; } = null!;
}

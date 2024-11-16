using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Language
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ProductCategoryLocalized> ProductCategoryLocalizeds { get; set; } = new List<ProductCategoryLocalized>();

    public virtual ICollection<ProductLocalized> ProductLocalizeds { get; set; } = new List<ProductLocalized>();

    public virtual ICollection<ProductRequestDataLocalized> ProductRequestDataLocalizeds { get; set; } = new List<ProductRequestDataLocalized>();
}

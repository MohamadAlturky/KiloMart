using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Language
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Faq> Faqs { get; set; } = new List<Faq>();

    public virtual ICollection<ProductCategoryLocalized> ProductCategoryLocalizeds { get; set; } = new List<ProductCategoryLocalized>();

    public virtual ICollection<ProductLocalized> ProductLocalizeds { get; set; } = new List<ProductLocalized>();

    public virtual ICollection<ProductRequestDataLocalized> ProductRequestDataLocalizeds { get; set; } = new List<ProductRequestDataLocalized>();

    public virtual ICollection<TextualConfig> TextualConfigs { get; set; } = new List<TextualConfig>();
}

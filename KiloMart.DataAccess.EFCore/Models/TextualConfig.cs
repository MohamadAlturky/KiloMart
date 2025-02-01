using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class TextualConfig
{
    public int Key { get; set; }

    public string Value { get; set; } = null!;

    public byte Language { get; set; }

    public virtual Language LanguageNavigation { get; set; } = null!;
}

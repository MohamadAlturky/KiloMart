using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Faq
{
    public int Id { get; set; }

    public string Question { get; set; } = null!;

    public string Answer { get; set; } = null!;

    public byte Language { get; set; }

    public byte Type { get; set; }

    public virtual Language LanguageNavigation { get; set; } = null!;
}

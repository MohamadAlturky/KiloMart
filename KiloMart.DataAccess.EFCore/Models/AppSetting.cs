using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class AppSetting
{
    public int Key { get; set; }

    public string Value { get; set; } = null!;
}

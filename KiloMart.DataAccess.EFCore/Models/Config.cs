using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Config
{
    public string Key { get; set; } = null!;

    public string Value { get; set; } = null!;
}

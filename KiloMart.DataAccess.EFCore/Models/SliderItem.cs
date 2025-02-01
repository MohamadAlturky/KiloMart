using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class SliderItem
{
    public int Id { get; set; }

    public string ImageUrl { get; set; } = null!;

    public int? Target { get; set; }

    public virtual ICollection<SliderItem> InverseTargetNavigation { get; set; } = new List<SliderItem>();

    public virtual SliderItem? TargetNavigation { get; set; }
}

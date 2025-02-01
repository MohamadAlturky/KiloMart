using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class MobileAppConfiguration
{
    public int Id { get; set; }

    public double CustomerAppMinimumBuildNumberAndroid { get; set; }

    public double CustomerAppMinimumBuildNumberIos { get; set; }

    public string CustomerAppUrlAndroid { get; set; } = null!;

    public string CustomerAppUrlIos { get; set; } = null!;

    public double ProviderAppMinimumBuildNumberAndroid { get; set; }

    public double ProviderAppMinimumBuildNumberIos { get; set; }

    public string ProviderAppUrlAndroid { get; set; } = null!;

    public string ProviderAppUrlIos { get; set; } = null!;

    public double DeliveryAppMinimumBuildNumberAndroid { get; set; }

    public double DeliveryAppMinimumBuildNumberIos { get; set; }

    public string DeliveryAppUrlAndroid { get; set; } = null!;

    public string DeliveryAppUrlIos { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class SystemSetting
{
    public int Id { get; set; }

    public double DeliveryOrderFee { get; set; }

    public double SystemOrderFee { get; set; }

    public bool CancelOrderWhenNoProviderHasAllProducts { get; set; }

    public int TimeInMinutesToMakeTheCircleBigger { get; set; }

    public double CircleRaduis { get; set; }

    public int MaxMinutesToCancelOrderWaitingAprovider { get; set; }

    public double MinOrderValue { get; set; }

    public double DistanceToAdd { get; set; }

    public double MaxDistanceToAdd { get; set; }

    public double RaduisForGetProducts { get; set; }
}

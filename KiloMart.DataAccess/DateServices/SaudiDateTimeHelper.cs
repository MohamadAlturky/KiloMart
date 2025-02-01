using System;

namespace KiloMart.Domain.DateServices;

public static class SaudiDateTimeHelper
{
    public static DateTime GetCurrentTime()
    {
        // Saudi Arabia Standard Time (UTC+03:00)
        TimeZoneInfo saudiTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, saudiTimeZone);
    }
}
using KiloMart.Domain.DateServices;
using Microsoft.Extensions.Caching.Memory;

namespace KiloMart.Presentation.Tracking;

public class DriversTrackerService
{
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheEntryOptions _cacheOptions;

    // Maintain a list of keys separately
    private readonly HashSet<int> _cacheKeys;

    public DriversTrackerService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30) // Cache expires in 30 seconds
        };
        _cacheKeys = new HashSet<int>();
    }

    /// <summary>
    /// Adds or updates the location of a driver.
    /// </summary>
    /// <param name="userId">The ID of the driver.</param>
    /// <param name="latitude">Latitude of the driver's location.</param>
    /// <param name="longitude">Longitude of the driver's location.</param>
    public void CreateOrUpdate(int userId, double latitude, double longitude)
    {
        var location = new DriverLocation
        {
            Latitude = latitude,
            Longitude = longitude,
            UpdatedAt = SaudiDateTimeHelper.GetCurrentTime()
        };

        _memoryCache.Set(userId, location, _cacheOptions);

        // Track the key
        _cacheKeys.Add(userId);
    }

    /// <summary>
    /// Gets the location of a driver by their user ID.
    /// </summary>
    /// <param name="userId">The ID of the driver.</param>
    /// <returns>DriverLocation object or null if not found.</returns>
    public DriverLocation? GetByKey(int userId)
    {
        _memoryCache.TryGetValue(userId, out DriverLocation? location);
        return location;
    }

    /// <summary>
    /// Gets all cached driver locations.
    /// </summary>
    /// <returns>A dictionary of user IDs and their locations.</returns>
    public Dictionary<int, DriverLocation> GetAll()
    {
        var allLocations = new Dictionary<int, DriverLocation>();

        foreach (var key in _cacheKeys)
        {
            if (_memoryCache.TryGetValue(key, out DriverLocation? location) && location != null)
            {
                allLocations[key] = location;
            }
        }

        return allLocations;
    }
    public HashSet<int> GetKeys()
    {
        return _cacheKeys;
    }

    /// <summary>
    /// Driver location data structure.
    /// </summary>
    public class DriverLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

using KiloMart.Core.Repositories;
using System.Collections.Concurrent;

namespace KiloMart.Core.Settings;

public interface IAppSettingsProvider
{
    Task<string?> GetSettingAsync(int key);
    Task UpdateSettingAsync(int key, string value);
    Task InvalidateCacheAsync();
}

public class AppSettingsProvider : IAppSettingsProvider
{
    private readonly IAppSettingsRepository _repository;
    private readonly ConcurrentDictionary<int, string> _cache;

    public AppSettingsProvider(IAppSettingsRepository repository)
    {
        _repository = repository;
        _cache = new ConcurrentDictionary<int, string>();
    }

    public async Task<string?> GetSettingAsync(int key)
    {
        if (_cache.TryGetValue(key, out string? cachedValue))
        {
            return cachedValue;
        }

        var setting = await _repository.GetSettingAsync(key);
        if (setting != null)
        {
            _cache.TryAdd(key, setting);
            return setting;
        }

        return null;
    }

    public async Task UpdateSettingAsync(int key, string value)
    {
        await _repository.UpdateSettingAsync(key, value);
        _cache.AddOrUpdate(key, value, (_, _) => value);
    }

    public Task InvalidateCacheAsync()
    {
        _cache.Clear();
        return Task.CompletedTask;
    }
}

using InventoryManagement.Interfaces.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace InventoryManagement.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var jsonData = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(jsonData))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(jsonData);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null)
    {
        if (value == null) return;

        var options = new DistributedCacheEntryOptions();
        
        if (absoluteExpireTime.HasValue)
        {
            options.SetAbsoluteExpiration(absoluteExpireTime.Value);
        }
        else if (slidingExpireTime.HasValue)
        {
            options.SetSlidingExpiration(slidingExpireTime.Value);
        }
        else
        {
            options.SetAbsoluteExpiration(TimeSpan.FromMinutes(5)); // Default 5 minutes
        }

        var jsonData = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, jsonData, options);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

    // A real production implementation would use StackExchange.Redis directly or IServer.Keys to remove by prefix
    // For simplicity with IDistributedCache, prefix removal might need tracked keys or specific clearing.
    // In a real scenario we'd track keys to remove them by pattern.
    public async Task RemoveByPrefixAsync(string prefixKey)
    {
        // Simple implementation - in real life use Redis pattern matching
        await Task.CompletedTask;
    }
}

using System.Collections.Concurrent;

namespace ProductManagement.Services
{
    public class RolePermissionCache : IRolePermissionCache
    {
        private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();

        public bool TryGetPermissions(string cacheKey, out IReadOnlyCollection<string> permissions)
        {
            permissions = Array.Empty<string>();

            if (!_cache.TryGetValue(cacheKey, out var cacheEntry))
            {
                return false;
            }

            if (cacheEntry.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                _cache.TryRemove(cacheKey, out _);
                return false;
            }

            permissions = cacheEntry.Permissions;
            return true;
        }

        public void SetPermissions(string cacheKey, IReadOnlyCollection<string> permissions, TimeSpan timeToLive)
        {
            var cacheEntry = new CacheEntry(
                [.. permissions],
                DateTimeOffset.UtcNow.Add(timeToLive));

            _cache[cacheKey] = cacheEntry;
        }

        private sealed record CacheEntry(IReadOnlyCollection<string> Permissions, DateTimeOffset ExpiresAt);
    }
}
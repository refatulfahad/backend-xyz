namespace ProductManagement.Services
{
    public interface IRolePermissionCache
    {
        bool TryGetPermissions(string cacheKey, out IReadOnlyCollection<string> permissions);

        void SetPermissions(string cacheKey, IReadOnlyCollection<string> permissions, TimeSpan timeToLive);
    }
}
namespace finance_management.Interfaces
{
    public interface IRedisService
    {
        Task SetAsync(string key, string value, TimeSpan? expiry = null);
        Task<string?> GetAsync(string key);
        Task<bool> ExistsAsync(string key);
        Task<bool> RemoveAsync(string key);
        Task SetObjectAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<T?> GetObjectAsync<T>(string key);
        Task<int> RemoveKeysByPatternAsync(string pattern);
    }
}

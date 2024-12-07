namespace SimpleRedisClient;

public interface IRedisClient
{
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<T?> GetAsync<T>(string key);
    Task<bool> DeleteAsync(string key);
    Task<bool> ExistsAsync(string key);

    // description: https://redis.io/commands/expire
    Task<bool> ExpireAsync(string key, TimeSpan expiry);
}
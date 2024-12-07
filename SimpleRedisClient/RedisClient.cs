using System.Text.Json;
using StackExchange.Redis;

namespace SimpleRedisClient;

public class RedisClient : IRedisClient//, IDisposable
{
    //private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _database;
    
    // public RedisClient(string connectionString)
    // {
    //     ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));
    //     
    //     _connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
    //     _database = _connectionMultiplexer.GetDatabase();
    // }
    
    public RedisClient(IConnectionMultiplexer redis)
    {
        //_connectionMultiplexer = redis;
        _database = redis.GetDatabase();
    }
    
    /// <summary>
    /// Set a value in the Redis database
    /// </summary>
    /// <param name="key">The key to set the value for</param>
    /// <param name="value">The value to set</param>
    /// <param name="expiry">The expiration time</param>
    /// <typeparam name="T">The type of the value</typeparam>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
        
        var serializedValue = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, serializedValue, expiry);
    }

    /// <summary>
    /// Get a value from the Redis database
    /// </summary>
    /// <param name="key">The key to get the value for</param>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <returns></returns>
    public async Task<T?> GetAsync<T>(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
        
        var serializedValue = await _database.StringGetAsync(key);
        return serializedValue.HasValue ? JsonSerializer.Deserialize<T>(serializedValue!) : default;
    }

    /// <summary>
    /// Delete a key from the Redis database
    /// </summary>
    /// <param name="key">The key to delete</param>
    /// <returns>True if the key was deleted, otherwise false</returns>
    public async Task<bool> DeleteAsync(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
        return await _database.KeyDeleteAsync(key);
    }

    /// <summary>
    /// Check if a key exists in the Redis database
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <returns>True if the key exists, otherwise false</returns>
    public async Task<bool> ExistsAsync(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
        return await _database.KeyExistsAsync(key);
    }

    /// <summary>
    /// Set a timeout on key. After the timeout has expired, the key will automatically be deleted.
    /// </summary>
    /// <param name="key">The key to set the expiration for</param>
    /// <param name="expiry">The expiration time</param>
    /// <returns></returns>
    public async Task<bool> ExpireAsync(string key, TimeSpan expiry)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
        return await _database.KeyExpireAsync(key, expiry);
    }

    // Dispose the connection multiplexer
    //public void Dispose() => _connectionMultiplexer.Dispose();
}
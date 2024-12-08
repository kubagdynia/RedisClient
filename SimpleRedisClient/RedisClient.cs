using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace SimpleRedisClient;

public class RedisClient : IRedisClient
{
    private readonly IDatabase _database;
    private readonly IConnectionMultiplexer _connection;
    private readonly ILogger<RedisClient> _logger;

    public RedisClient(IConnectionMultiplexer redis, ILogger<RedisClient> logger)
    {
        _logger = logger;
        _connection = redis;
        _database = redis.GetDatabase();
        
        AddConnectionEventHandlers();
    }

    /// <summary>
    /// Check if the client is connected to the Redis database
    /// </summary>
    public bool IsConnected => _connection.IsConnected;

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
    /// <returns>The value if it exists, otherwise null</returns>
    public async Task<T?> GetAsync<T>(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
        
        RedisValue serializedValue = await _database.StringGetAsync(key);
        return serializedValue.HasValue ? JsonSerializer.Deserialize<T>(serializedValue!) : default;
    }
    
    /// <summary>
    /// Try to get a value from the Redis database
    /// </summary>
    /// <param name="key">The key to get the value for</param>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <returns>True if the value was retrieved, otherwise false. The value if it exists, otherwise null</returns>
    public async Task<(bool Success, T? Value)> TryGetAsync<T>(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
        
        try
        {
            var redisValue = await _database.StringGetAsync(key);

            if (!redisValue.HasValue)
            {
                return (false, default); // Key does not exist
            }

            var value = JsonSerializer.Deserialize<T>(redisValue!);
            return (true, value); // Key exists and value was deserialized
        }
        catch
        {
            return (false, default); // In case of any exception
        }
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
    /// <returns>True if the timeout was set, otherwise false</returns>
    public async Task<bool> ExpireAsync(string key, TimeSpan expiry)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
        return await _database.KeyExpireAsync(key, expiry);
    }
    
    private void AddConnectionEventHandlers()
    {
        _connection.ConnectionFailed += (sender, args)
            => _logger.LogError($"Redis connection failed: {args.Exception}");
        
        _connection.ConnectionRestored += (sender, args)
            => _logger.LogInformation($"Redis connection restored (type: {args.ConnectionType}, failure: {args.FailureType})");
        
        // _connection.ErrorMessage += (sender, args)
        //     => _logger.LogError($"Redis error: {args.Message}");
        //
        // _connection.InternalError += (sender, args) 
        //     => _logger.LogError($"Redis internal error: {args.Exception}");
    }
}
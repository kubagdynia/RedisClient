namespace SimpleRedisClient;

public interface IRedisClient
{
    /// <summary>
    /// Check if the client is connected to the Redis database
    /// </summary>
    bool IsConnected { get; }
    
    /// <summary>
    /// Set a value in the Redis database
    /// </summary>
    /// <param name="key">The key to set the value for</param>
    /// <param name="value">The value to set</param>
    /// <param name="expiry">The expiration time</param>
    /// <typeparam name="T">The type of the value</typeparam>
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    
    /// <summary>
    /// Get a value from the Redis database
    /// </summary>
    /// <param name="key">The key to get the value for</param>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <returns>The value if it exists, otherwise null</returns>
    Task<T?> GetAsync<T>(string key);
    
    /// <summary>
    /// Try to get a value from the Redis database
    /// </summary>
    /// <param name="key">The key to get the value for</param>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <returns>True if the value was retrieved, otherwise false. The value if it exists, otherwise null</returns>
    Task<(bool Success, T? Value)> TryGetAsync<T>(string key);
    
    /// <summary>
    /// Delete a key from the Redis database
    /// </summary>
    /// <param name="key">The key to delete</param>
    /// <returns>True if the key was deleted, otherwise false</returns>
    Task<bool> DeleteAsync(string key);
    
    /// <summary>
    /// Check if a key exists in the Redis database
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <returns>True if the key exists, otherwise false</returns>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// Set a timeout on key. After the timeout has expired, the key will automatically be deleted.
    /// </summary>
    /// <param name="key">The key to set the expiration for</param>
    /// <param name="expiry">The expiration time</param>
    /// <returns>True if the timeout was set, otherwise false</returns>
    Task<bool> ExpireAsync(string key, TimeSpan expiry);
}
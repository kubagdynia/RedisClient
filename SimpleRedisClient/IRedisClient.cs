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
    /// Set multiple values in the Redis database
    /// </summary>
    /// <param name="items">The key-value pairs to set</param>
    /// <param name="expiry">The expiration time</param>
    /// <typeparam name="T">The type of the values</typeparam>
    /// <exception cref="ArgumentException">Thrown if the key-value pairs are null or empty</exception>
    Task SetAsync<T>(IDictionary<string, T> items, TimeSpan? expiry = null);
    
    /// <summary>
    /// Set multiple values in the Redis database
    /// </summary>
    /// <param name="items">The key-value pairs to set</param>
    /// <typeparam name="T">The type of the values</typeparam>
    /// <exception cref="ArgumentException">Thrown if the key-value pairs are null or empty</exception>
    Task SetAsync<T>(IDictionary<string, (T Value, TimeSpan? Expiry)> items);
    
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
    /// Get the remaining time to live for a key
    /// </summary>
    /// <param name="key">The key to get the remaining time to live for</param>
    /// <returns>The remaining time to live for the key, or null if the key does not exist or does not have a timeout</returns>
    /// <exception cref="ArgumentException">Thrown if the key is null or whitespace</exception>
    Task<TimeSpan?> GetRemainingTtlAsync(string key);
    
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

    /// <summary>
    /// Publish a message to a channel
    /// </summary>
    /// <param name="channel">The channel to publish the message to</param>
    /// <param name="message">The message to publish</param>
    /// <returns>The number of clients that received the message</returns>
    Task<long> PublishAsync(string channel, string message);

    void Subscribe(string channel, Action<string> messageHandler);
}
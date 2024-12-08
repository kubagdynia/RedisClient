using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleRedisClient.Configurations;
using StackExchange.Redis;

namespace SimpleRedisClient.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSimpleRedisClient(this IServiceCollection services, IConfiguration config)
    {
        RedisSettings redisSettings = config.GetSection(RedisSettings.SectionName).Get<RedisSettings>()!;
        return AddSimpleRedisClient(services, redisSettings.ConnectionString);
    }
    
    public static IServiceCollection AddSimpleRedisClient(this IServiceCollection services, string connectionString)
    {
        // var configuration = new ConfigurationOptions
        // {
        //     AbortOnConnectFail = false, // Do not abort if the connection fails
        //     EndPoints = { connectionString },
        //     ConnectTimeout = 5000, // 5 seconds
        //     SyncTimeout = 5000, // 5 seconds
        //     ConnectRetry = 3, // Retry 3 times
        //     ReconnectRetryPolicy = new ExponentialRetry(2000), // Retry every 2 seconds
        // };
        
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connectionString);
        //ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(configuration);
        services.AddSingleton<IConnectionMultiplexer>(redis);
        services.AddSingleton<IRedisClient, RedisClient>();
        return services;
    }
    
    public static IServiceCollection AddSimpleRedisClient(this IServiceCollection services, IOptions<RedisSettings> options)
    {
        return AddSimpleRedisClient(services, options.Value.ConnectionString);
    }
}
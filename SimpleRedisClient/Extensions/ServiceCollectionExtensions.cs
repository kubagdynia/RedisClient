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
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connectionString);
        services.AddSingleton<IConnectionMultiplexer>(redis);
        services.AddSingleton<IRedisClient, RedisClient>();
        return services;
    }
    
    public static IServiceCollection AddSimpleRedisClient(this IServiceCollection services, IOptions<RedisSettings> options)
    {
        return AddSimpleRedisClient(services, options.Value.ConnectionString);
    }
}
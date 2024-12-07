namespace SimpleRedisClient.Configurations;

public record RedisSettings
{
    public const string SectionName = "Redis";
    public string ConnectionString { get; init; } = "localhost:6379,abortConnect=False";
};
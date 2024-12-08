using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleRedisClient;
using SimpleRedisClient.Extensions;

var services = new ServiceCollection();

// register services
services.AddLogging(logging =>
{
    logging.AddConsole(); // Add console logger
});

services.AddSimpleRedisClient("localhost:6379"); // need to have redis server running

// build service provider
var serviceProvider = services.BuildServiceProvider();

// get redis client
var redisClient = serviceProvider.GetRequiredService<IRedisClient>();

Console.WriteLine($"Redis client is connected: {redisClient.IsConnected}");

string key = "myKey";

var (success, value) = await redisClient.TryGetAsync<string>(key);

if (success)
{
    Console.WriteLine($"Key found! Value: {value}");
}
else
{
    Console.WriteLine($"Key {key} does not exist");
    // set a value
    await redisClient.SetAsync(key, "my super value");
}
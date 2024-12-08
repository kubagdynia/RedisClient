using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;

namespace SimpleRedisClient.Tests;

public class RedisClientTests
{
    private Mock<IDatabase> _databaseMock;
    private Mock<IConnectionMultiplexer> _connectionMock;
    private IRedisClient _redisClient;
    
    [SetUp]
    public void Setup()
    {
        // Mock the database and connection
        _databaseMock = new Mock<IDatabase>();
        _connectionMock = new Mock<IConnectionMultiplexer>();
        _connectionMock.Setup(c => c.GetDatabase(It.IsAny<int>(), null))
            .Returns(_databaseMock.Object);
        _connectionMock.Setup(c => c.IsConnected)
            .Returns(true);
        
        var loggerMock = new Mock<ILogger<RedisClient>>();
        
        // Inject the mocks into the RedisClient
        _redisClient = new RedisClient(_connectionMock.Object, loggerMock.Object);
    }
    
    [Test]
    public void IsConnected_ShouldReturnTrue()
    {
        // Act
        bool isConnected = _redisClient.IsConnected;
        
        // Assert
        Assert.That(isConnected, Is.True);
    }

    [Test]
    public async Task SetAsync_ShouldCallStringSetAsync()
    {
        // Arrange
        var key = "testKey";
        var value = "testValue";

        // Act
        await _redisClient.SetAsync(key, value);

        // Assert
        _databaseMock.Verify(db => db.StringSetAsync(key, It.IsAny<RedisValue>(), null, false, When.Always, CommandFlags.None), Times.Once);
    }
    
    [Test]
    public async Task GetAsync_ShouldReturnDeserializedValue()
    {
        // Arrange
        string key = "testKey";
        string value = "testValue";
        _databaseMock.Setup(db => db.StringGetAsync(key, CommandFlags.None)).ReturnsAsync(JsonSerializer.Serialize(value));

        // Act
        string? result = await _redisClient.GetAsync<string>(key);

        // Assert
        Assert.That(value, Is.EqualTo(result));
    }

    [Test]
    public async Task RemoveAsync_ShouldCallKeyDeleteAsync()
    {
        // Arrange
        string key = "testKey";

        // Act
        await _redisClient.DeleteAsync(key);

        // Assert
        _databaseMock.Verify(db => db.KeyDeleteAsync(key, CommandFlags.None), Times.Once);
    }

    [Test]
    public async Task ExistsAsync_ShouldCallKeyExistsAsync()
    {
        // Arrange
        string key = "testKey";

        // Act
        await _redisClient.ExistsAsync(key);

        // Assert
        _databaseMock.Verify(db => db.KeyExistsAsync(key, CommandFlags.None), Times.Once);
    }
}
using CashFlow.Infrastructure.Shared.Caching;
using CashFlow.Shared.Serialization;
using Moq;
using StackExchange.Redis;

namespace CashFlow.Infrastructure.Shared.Tests.Caching
{
    public class RedisCacheServiceTests
    {
        private readonly Mock<IConnectionMultiplexer> _redis = new();
        private readonly Mock<IDatabase> _database = new();
        private readonly Mock<IServer> _server = new();
        private readonly Mock<IJsonSerializerService> _jsonSerializer = new();

        public RedisCacheServiceTests()
        {
            _redis.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_database.Object);
            _redis.Setup(x => x.GetEndPoints(It.IsAny<bool>())).Returns(new[] { new System.Net.DnsEndPoint("localhost", 6379) });
            _redis.Setup(x => x.GetServer(It.IsAny<System.Net.EndPoint>(), null)).Returns(_server.Object);
        }

        private RedisCacheService CreateService() => new RedisCacheService(_redis.Object, _jsonSerializer.Object);

        [Fact]
        public async Task GetItemsAsync_Should_Return_Deserialized_List_When_Json_Is_Array()
        {
            // Arrange
            var service = CreateService();
            var key = "key1";
            var value = "[{\"Name\":\"A\"},{\"Name\":\"B\"}]";
            var redisValue = new RedisValue(value);

            _database.Setup(db => db.StringGetAsync(key, CommandFlags.None))
                .ReturnsAsync(redisValue);

            _jsonSerializer.Setup(j => j.Deserialize<IEnumerable<SampleClass>>(value))
                .Returns(new List<SampleClass> { new SampleClass { Name = "A" }, new SampleClass { Name = "B" } });

            // Act
            var result = await service.GetItemsAsync<SampleClass>(key);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetItemsAsync_Should_Return_Deserialized_Single_When_Json_Is_Object()
        {
            // Arrange
            var service = CreateService();
            var key = "key1";
            var value = "{\"Name\":\"C\"}";
            var redisValue = new RedisValue(value);

            _database.Setup(db => db.StringGetAsync(key, CommandFlags.None))
                .ReturnsAsync(redisValue);

            _jsonSerializer.Setup(j => j.Deserialize<SampleClass>(value))
                .Returns(new SampleClass { Name = "C" });

            // Act
            var result = await service.GetItemsAsync<SampleClass>(key);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("C", result.First().Name);
        }

        [Fact]
        public async Task GetItemsAsync_Should_Return_Null_When_Value_Is_Empty()
        {
            // Arrange
            var service = CreateService();
            var key = "key1";
            _database.Setup(db => db.StringGetAsync(key, CommandFlags.None))
                .ReturnsAsync(RedisValue.Null);

            // Act
            var result = await service.GetItemsAsync<SampleClass>(key);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_Should_Call_KeyDeleteAsync()
        {
            // Arrange
            var service = CreateService();
            var key = "key-to-delete";

            // Act
            await service.DeleteAsync(key);

            // Assert
            _database.Verify(db => db.KeyDeleteAsync(key, CommandFlags.None), Times.Once);
        }

        [Fact]
        public async Task GetItemsByPatternAsync_Should_Return_Deserialized_Items()
        {
            // Arrange
            var service = CreateService();
            var pattern = "pattern*";
            var keys = new RedisKey[] { "key1", "key2" };
            var values = new RedisValue[] { "[{\"Name\":\"D\"}]", "{\"Name\":\"E\"}" };

            _server.Setup(s => s.Keys(
            It.IsAny<int>(),                      // database
            pattern,                              // pattern
            It.IsAny<int>(),                      // pageSize
            It.IsAny<long>(),                     // cursor
            It.IsAny<int>(),                      // pageOffset
            It.IsAny<CommandFlags>()              // flags
        )).Returns(keys.Select(k => (RedisKey)k));


            _database.Setup(d => d.StringGetAsync(keys, CommandFlags.None)).ReturnsAsync(values);

            _jsonSerializer.Setup(j => j.Deserialize<IEnumerable<SampleClass>>("[{\"Name\":\"D\"}]"))
                .Returns(new List<SampleClass> { new SampleClass { Name = "D" } });

            _jsonSerializer.Setup(j => j.Deserialize<SampleClass>("{\"Name\":\"E\"}"))
                .Returns(new SampleClass { Name = "E" });

            // Act
            var result = await service.GetItemsByPatternAsync<SampleClass>(pattern);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.Name == "D");
            Assert.Contains(result, r => r.Name == "E");
        }

        [Fact]
        public async Task GetAllItemsAsync_Should_Return_Empty_If_No_Keys()
        {
            // Arrange
            var service = CreateService();
            var keys = new RedisKey[0];

            _server.Setup(s => s.Keys(
                It.IsAny<int>(),
                It.IsAny<RedisValue>(),
                It.IsAny<int>(),
                It.IsAny<long>(),
                It.IsAny<int>(),
                It.IsAny<CommandFlags>()
            )).Returns(keys);

            // Act
            var result = await service.GetAllItemsAsync<SampleClass>();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        public class SampleClass
        {
            public string Name { get; set; } = "";
        }
    }
}
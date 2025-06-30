using CashFlow.BuildingBlocks.Domain.Caching;
using CashFlow.Shared.Serialization;
using StackExchange.Redis;

namespace CashFlow.Infrastructure.Shared.Caching
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly IJsonSerializerService _jsonSerializer;

        public RedisCacheService(IConnectionMultiplexer redis, IJsonSerializerService jsonSerializer)
        {
            _redis = redis;
            _database = redis.GetDatabase();
            _jsonSerializer = jsonSerializer;
        }

        public async Task AddOrUpdateAsync<T>(string key, T item)
        {
            var json = _jsonSerializer.Serialize(item);
            await _database.StringSetAsync(key, json);
        }

        public async Task<IEnumerable<T>?> GetItemsAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);
            if (value.IsNullOrEmpty) return null;

            if (value.ToString().TrimStart().StartsWith("["))
                return _jsonSerializer.Deserialize<IEnumerable<T>>(value!);
            else
                return new List<T> { _jsonSerializer.Deserialize<T>(value!)! };
        }

        public async Task<IEnumerable<T>?> GetItemsByPatternAsync<T>(string pattern)
        {
            var endpoints = _redis.GetEndPoints();
            var server = _redis.GetServer(endpoints.First());

            var keys = server.Keys(database: _database.Database, pattern: pattern).ToArray();
            if (keys == null || keys.Length == 0)
                return Enumerable.Empty<T>();

            var values = await _database.StringGetAsync(keys);
            var result = new List<T>();

            foreach (var value in values)
            {
                if (!value.IsNullOrEmpty)
                {
                    if (value.ToString().TrimStart().StartsWith("["))
                        result.AddRange(_jsonSerializer.Deserialize<IEnumerable<T>>(value!)!);
                    else
                        result.Add(_jsonSerializer.Deserialize<T>(value!)!);
                }
            }

            return result;
        }
        public async Task<IEnumerable<T>?> GetAllItemsAsync<T>()
        {
            var endpoints = _redis.GetEndPoints();
            var server = _redis.GetServer(endpoints.First());

            var keys = server.Keys(database: _database.Database, pattern: "*").ToArray();
            if (keys == null || keys.Length == 0)
                return Enumerable.Empty<T>();

            var values = await _database.StringGetAsync(keys);
            var result = new List<T>();

            foreach (var value in values)
            {
                if (!value.IsNullOrEmpty)
                {
                    if (value.ToString().TrimStart().StartsWith("["))
                        result.AddRange(_jsonSerializer.Deserialize<IEnumerable<T>>(value!)!);
                    else
                        result.Add(_jsonSerializer.Deserialize<T>(value!)!);
                }
            }

            return result;
        }

        public async Task DeleteAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }
    }
}
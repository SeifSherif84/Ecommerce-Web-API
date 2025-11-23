using StackExchange.Redis;
using Store.G02.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Store.G02.Persistence.Repositories
{
    public class CacheRepository(IConnectionMultiplexer Redis) : ICacheRepository
    {
        private readonly IDatabase _database = Redis.GetDatabase();

        public async Task<string?> GetAsync(string key)
        {
            var value = await _database.StringGetAsync(key);
            return value;
        }

        public async Task SetAsync(string key, object value, TimeSpan duration)
        {
            await _database.StringSetAsync(key, JsonSerializer.Serialize(value), duration);
        }
    }
}

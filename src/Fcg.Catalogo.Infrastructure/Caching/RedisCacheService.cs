using Fcg.Catalogo.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fcg.Catalogo.Infrastructure.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly string _instanceName = "FiapCloudGames:";

        public RedisCacheService(IDistributedCache cache, IConnectionMultiplexer redisConnection)
        {
            _cache = cache;
            _redisConnection = redisConnection;
        }


        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task DefinirAsync<T>(string chave, T valor, TimeSpan tempoExpiracao)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = tempoExpiracao
            };

            var jsonData = JsonSerializer.Serialize(valor);
            await _cache.SetStringAsync(chave, jsonData, options);
        }

        public async Task<T?> ObterAsync<T>(string chaveCache)
        {
            var cacheData = await _cache.GetStringAsync(chaveCache);
            if (string.IsNullOrEmpty(cacheData))
                return default;

            return JsonSerializer.Deserialize<T>(cacheData, _jsonOptions);
        }

        public async Task RemoverAsync(string chaveCache)
        {
            await _cache.RemoveAsync(chaveCache);
        }

        public async Task RemoverPorPrefixoAsync(string prefixo)
        {
            var endpoints = _redisConnection.GetEndPoints();
            var server = _redisConnection.GetServer(endpoints.First());
            var chaves = server.Keys(pattern: $"{_instanceName}{prefixo}*").ToArray();

            if (chaves.Any())
            {
                var db = _redisConnection.GetDatabase();
                await db.KeyDeleteAsync(chaves);
            }
        }
    }
}

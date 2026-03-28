using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ReportBroker.Application.Interfaces;
using System.Text.Json.Serialization;

namespace ReportBroker.Infrastructure.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        {
            var data = await _cache.GetStringAsync(key, ct);

            if (data == null)
                return default;

            return JsonConvert.DeserializeObject<T>(data);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan exp, CancellationToken ct = default)
        {
            var options = new DistributedCacheEntryOptions
            {        
                AbsoluteExpirationRelativeToNow = exp
            };

            var data = JsonConvert.SerializeObject(value);
            await _cache.SetStringAsync(key, data, options, ct);
        }

        public async Task RemoveAsync(string key, CancellationToken ct = default)
        {
            await _cache.RemoveAsync(key, ct);
        }


    }
}

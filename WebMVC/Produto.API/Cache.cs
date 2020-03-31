using Newtonsoft.Json;
using Produto.API.Model;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Produto.API
{
    public class Cache<T>
    {

        // Redis Connection string info
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            //string cacheConnection = ConfigurationManager.AppSettings["CacheConnection"].ToString();
            string cacheConnection = "localhost";
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        public static ConnectionMultiplexer Connection => lazyConnection.Value;

        // Set five minute expiration as a default
        private const double DefaultExpirationTimeInMinutes = 5.0;

        public async Task SetAsync(string key, T myItem)
        {
            var cache = Connection.GetDatabase();

            await cache.StringSetAsync(key, JsonConvert.SerializeObject(myItem)).ConfigureAwait(false);
            await cache.KeyExpireAsync(key, TimeSpan.FromMinutes(DefaultExpirationTimeInMinutes)).ConfigureAwait(false);

        }

        public async Task<T> GetAsync(string pkey)
        {
            // Define a unique key for this method and its parameters.
            var key = $"{pkey}";
            var cache = Connection.GetDatabase();

            // Try to get the entity from the cache.
            var json = await cache.StringGetAsync(key).ConfigureAwait(false);
            var value = string.IsNullOrWhiteSpace(json)
                          ? default(T)
                          : JsonConvert.DeserializeObject<T>(json);

            return value;
        }
    }
}

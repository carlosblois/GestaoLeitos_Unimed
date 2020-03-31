using System;

namespace CacheRedis
{
    using Newtonsoft.Json;
    using StackExchange.Redis;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class Cache<T>
    {
        static string _cacheConnection;
        static double  _ExpirationTimeInMinutes;

        public Cache(string cacheConnection, double ExpirationTimeInMinutes)
        {
            _cacheConnection = cacheConnection;
            _ExpirationTimeInMinutes = ExpirationTimeInMinutes;
        }

        // Redis Connection string info
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(_cacheConnection);
        });

        public static ConnectionMultiplexer Connection => lazyConnection.Value;

        // Set five minute expiration as a default
        private const double DefaultExpirationTimeInMinutes = 5.0;


        public async Task SetAsync(string key, T myItem)
        {
            var cache = Connection.GetDatabase();                   
            await cache.StringSetAsync(key, JsonConvert.SerializeObject(myItem)).ConfigureAwait(false);
            await cache.KeyExpireAsync(key, TimeSpan.FromMinutes(_ExpirationTimeInMinutes)).ConfigureAwait(false);

        }

        public async Task DelAsync(string key, T myItem)
        {
            var cache = Connection.GetDatabase();
            await cache.KeyDeleteAsync (key).ConfigureAwait(false);
        }

        public async Task<Boolean> UpdateListAsync(string key)
        {
            var cache = Connection.GetDatabase();
            return await cache.KeyDeleteAsync(key);
        }

        public async Task<Boolean> DelListAsync(string key)
        {
            var cache = Connection.GetDatabase();
            return await cache.KeyDeleteAsync(key);
        }

        public async Task<List<T>> GetListAsync(string key)
        {
            var cache = Connection.GetDatabase();
            long count = cache.ListLength(key);
            List<T> lst = new List<T>();
            for (long i = 0; i < count; i++)
            {
                var lstit = await cache.ListGetByIndexAsync(key, i);
                T it = JsonConvert.DeserializeObject<T>(lstit);
                lst.Add(it);
            }
            return lst;
        }

        public async Task SetListAsync(string key, List<T> myItems)
        {
            var cache = Connection.GetDatabase();

            foreach (T it in myItems)
            {
                await cache.ListRightPushAsync(key, JsonConvert.SerializeObject(it));
            }
            await cache.KeyExpireAsync(key, TimeSpan.FromMinutes(_ExpirationTimeInMinutes)).ConfigureAwait(false);
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



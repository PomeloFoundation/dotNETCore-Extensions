using System;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;

namespace Microsoft.Extensions.Caching.Distributed
{
    public static class DistributedCacheExtensions
    {
        public static T GetObject<T>(this IDistributedCache self, string key)
        {
            var str = self.GetString(key);
            if (str == null)
                return default(T);
            else
                return JsonConvert.DeserializeObject<T>(str);
        }

        public static async Task<T> GetObjectAsync<T>(this IDistributedCache self, string key)
        {
            var str = await self.GetStringAsync(key);
            if (str == null)
                return default(T);
            else
                return JsonConvert.DeserializeObject<T>(str);
        }

        public static void SetObject<T>(this IDistributedCache self, string key, T obj, DistributedCacheEntryOptions options = null)
        {
            if (options == null)
                self.SetString(key, JsonConvert.SerializeObject(obj));
            else
                self.SetString(key, JsonConvert.SerializeObject(obj), options);
        }

        public static Task SetObjectAsync<T>(this IDistributedCache self, string key, T obj, DistributedCacheEntryOptions options = null)
        {
            if (options == null)
                return self.SetStringAsync(key, JsonConvert.SerializeObject(obj));
            else
                return self.SetStringAsync(key, JsonConvert.SerializeObject(obj), options);
        }

        public static void SetObject<T>(this IDistributedCache self, string key, Action<T> setup, DistributedCacheEntryOptions options = null)
            where T : class, new()
        {
            T obj;
            var type = typeof(T).GetTypeInfo();
            var str = self.GetString(key);
            if (str == null)
            {
                obj = new T();
            }
            else
            {
                obj = (T)JsonConvert.DeserializeObject(str);
            }
            setup(obj);
            if (options == null)
                self.SetString(key, JsonConvert.SerializeObject(obj));
            else
                self.SetString(key, JsonConvert.SerializeObject(obj), options);
        }

        public static void SetObject<T>(this IDistributedCache self, string key, Func<T, T> setup, DistributedCacheEntryOptions options = null)
            where T : struct
        {
            T obj;
            var type = typeof(T).GetTypeInfo();
            var str = self.GetString(key);
            if (str == null)
            {
                obj = new T();
            }
            else
            {
                obj = (T)JsonConvert.DeserializeObject(str);
            }
            obj = setup(obj);
            if (options == null)
                self.SetString(key, JsonConvert.SerializeObject(obj));
            else
                self.SetString(key, JsonConvert.SerializeObject(obj), options);
        }

        public static async Task SetObjectAsync<T>(this IDistributedCache self, string key, Action<T> setup, DistributedCacheEntryOptions options = null)
            where T : class, new()
        {
            T obj;
            var type = typeof(T).GetTypeInfo();
            var str = await self.GetStringAsync(key);
            if (str == null)
            {
                obj = new T();
            }
            else
            {
                obj = (T)JsonConvert.DeserializeObject(str);
            }
            setup(obj);
            if (options == null)
                await self.SetStringAsync(key, JsonConvert.SerializeObject(obj));
            else
                await self.SetStringAsync(key, JsonConvert.SerializeObject(obj), options);
        }

        public static async Task SetObjectAsync<T>(this IDistributedCache self, string key, Func<T, T> setup, DistributedCacheEntryOptions options = null)
            where T : struct
        {
            T obj;
            var type = typeof(T).GetTypeInfo();
            var str = await self.GetStringAsync(key);
            if (str == null)
            {
                obj = new T();
            }
            else
            {
                obj = (T)JsonConvert.DeserializeObject(str);
            }
            obj = setup(obj);
            if (options == null)
                await self.SetStringAsync(key, JsonConvert.SerializeObject(obj));
            else
                await self.SetStringAsync(key, JsonConvert.SerializeObject(obj), options);
        }
    }
}

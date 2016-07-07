using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace Pomelo.AspNetCore.Localization
{
    public class MemoryTranslatedCaching : ITranslatedCaching
    {
        private IMemoryCache Cache { get; set; }

        public MemoryTranslatedCaching(IMemoryCache Cache)
        {
            this.Cache = Cache;
        }

        public string Get(string key, string culture)
        {
            IDictionary<string, string> value;
            var flag = Cache.TryGetValue(key, out value);
            if (!flag) return null;
            if (value.ContainsKey(culture))
                return value[culture];
            else
                return null;
        }

        public void Set(string key, string culture, string dst)
        {
            IDictionary<string, string> value;
            var flag = Cache.TryGetValue(key, out value);
            if (!flag)
            {
                value = new Dictionary<string, string>();
                value.Add(culture, dst);
                Cache.Set(key, value);
            }
            else
            {
                if (value.ContainsKey(culture))
                    value[culture] = dst;
                else
                    value.Add(culture, dst);
            }
        }
    }
}

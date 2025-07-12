using AdventureWorks.Infrastructure.CacheProvider.BaseCache.Interface;
using AdventureWorks.Infrastructure.CacheProvider.BaseCache;
using System.Runtime.Caching;

namespace AdventureWorks.Infrastructure.CacheProvider.MemCache
{
    public class MemCacheProvider : BaseCacheProvider, ICacheProvider, IFallBackCacheProvider
    {
        private ObjectCache Cache
        {
            get { return MemoryCache.Default; }
        }
        public override List<string> GetAllKey()
        {
            var cacheKeys = new List<string>();
            cacheKeys = Cache.Select(kvp => kvp.Key).ToList();
            return cacheKeys;
        }
        public override T Get<T>(string key)
        {
            return (T)Cache[key];
        }
        public override object Get(string key)
        {
            return Cache[key];
        }
        public override void Set(string key, object data, double cacheTime)
        {
            if (data != null)
            {
                var policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime);

                if (!string.IsNullOrEmpty(key))
                    Cache.Add(new CacheItem(key, data), policy);
            }
        }
        public override bool IsSet(string key)
        {
            return (Cache[key] != null);
        }
        public override void Remove(string cacheKey)
        {
            try
            {
                Cache.Remove(cacheKey);
            }
            catch { }
        }
    }
}

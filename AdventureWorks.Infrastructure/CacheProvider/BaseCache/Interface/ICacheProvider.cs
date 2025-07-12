using System.Threading.Tasks;

namespace AdventureWorks.Infrastructure.CacheProvider.BaseCache.Interface
{
    public interface ICacheProvider
    {
        List<string> GetAllKey();
        object Get(string key);
        T Get<T>(string key);
        void Set(string key, object? data, double cacheTime);
        void Set(string key, object? data, int cacheTime);
        bool IsSet(string key);
        void Invalidate(string key);
        void RemoveAll();
        void RemoveByPattern(string key);
        string BuildCachedKey(params object?[] objects);
        public T? GetOrSet<T>(Func<T?> getDataSource,
           string key,
           double cacheTime = CachingTime.CACHING_TIME_DEFAULT_IN_1_MINUTES,
           bool isDeepClone = true)
           where T : class;
        public Task<T?> GetOrSet<T>(Func<Task<T?>> getDataSource, 
            string key, 
            double cacheTime = 1,
            bool isDeepClone = true) 
            where T : class;

        void Remove(string cacheKey);
        void Remove(List<string> cacheKeys);
    }
}

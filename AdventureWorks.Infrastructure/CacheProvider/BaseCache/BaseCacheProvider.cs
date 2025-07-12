using AdventureWorks.Infrastructure.CacheProvider.BaseCache.Interface;
using Force.DeepCloner;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorks.Infrastructure.CacheProvider.BaseCache
{
    public abstract class BaseCacheProvider : ICacheProvider
    {
        public abstract object Get(string key);
        public abstract T Get<T>(string key);
        public abstract List<string> GetAllKey();
        public abstract bool IsSet(string key);
        public string BuildCachedKey(params object?[] objects)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (objects != null)
            {
                foreach (var item in objects)
                {
                    stringBuilder.AppendFormat("{0}_", item);
                }
            }
            return stringBuilder.ToString();
        }
        public abstract void Remove(string cacheKey);
        public abstract void Set(string key, object? data, double cacheTime);
        public virtual void Set(string key, object data, int cacheTime)
        {
            Set(key, data, (double)cacheTime);
        }
        public virtual void Invalidate(string key)
        {
            Remove(key);
        }
        public virtual void RemoveAll()
        {
            Remove(GetAllKey());
        }
        public virtual void Remove(List<string> cacheKeys)
        {
            try
            {
                foreach (string cacheKey in cacheKeys)
                {
                    Remove(cacheKey);
                }
            }
            catch { }
        }
        public virtual void RemoveByPattern(string name)
        {
            try
            {
                List<string> cacheKeys =
                    GetAllKey().Where(key => key.ToLower().Contains(name.ToLower())).ToList();
                foreach (string cacheKey in cacheKeys)
                {
                    Remove(cacheKey);
                }
            }
            catch { }
        }
        public virtual T? GetOrSet<T>(Func<T?> getDataSource,
               string key,
               double cacheTime = CachingTime.CACHING_TIME_DEFAULT_IN_5_MINUTES,
               bool isDeepClone = true)
        {
            T? result = default;
            if (!IsSet(key))
            {
                result = getDataSource.Invoke();
                Set(key, result, cacheTime);
            }
            else
            {
                try
                {
                    result = Get<T>(key);
                }
                catch
                {
                    Invalidate(key);
                }
            }

            return isDeepClone && result != null ? result.DeepClone() : result;
        }

        public virtual async Task<T?> GetOrSet<T>(Func<Task<T?>> getDataSource, string key, double cacheTime = 1, bool isDeepClone = true)
        {
            T? result = default;
            if (!IsSet(key))
            {
                result = await getDataSource.Invoke();
                Set(key, result, cacheTime);
            }
            else
            {
                try
                {
                    result = Get<T>(key);
                }
                catch
                {
                    Invalidate(key);
                }
            }

            return isDeepClone && result != null ? result.DeepClone() : result;
        }
    }
}

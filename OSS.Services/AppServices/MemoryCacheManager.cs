using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace OSS.Services.AppServices
{
    /// <summary>
    /// In-memory cache manager backed by IMemoryCache.
    /// Implements both ICacheManager and ILocker via a single shared instance
    /// (register both interfaces pointing to the same singleton — Fix-8).
    /// </summary>
    public class MemoryCacheManager : ILocker, ICacheManager
    {
        private readonly IMemoryCache _cache;
        private static readonly ConcurrentDictionary<string, bool> _allKeys = new();
        private CancellationTokenSource _cancellationTokenSource = new();

        public MemoryCacheManager(IMemoryCache cache)
        {
            _cache = cache;
        }

        private MemoryCacheEntryOptions GetMemoryCacheEntryOptions(TimeSpan cacheTime)
        {
            var options = new MemoryCacheEntryOptions()
                .AddExpirationToken(new CancellationChangeToken(_cancellationTokenSource.Token))
                .RegisterPostEvictionCallback(PostEviction);
            options.AbsoluteExpirationRelativeToNow = cacheTime;
            return options;
        }

        private string AddKey(string key)
        {
            _allKeys.TryAdd(key, true);
            return key;
        }

        private string RemoveKey(string key)
        {
            TryRemoveKey(key);
            return key;
        }

        private void TryRemoveKey(string key)
        {
            if (!_allKeys.TryRemove(key, out _))
                _allKeys.TryUpdate(key, false, true);
        }

        private void ClearKeys()
        {
            foreach (var key in _allKeys.Where(p => !p.Value).Select(p => p.Key).ToList())
                RemoveKey(key);
        }

        private void PostEviction(object key, object? value, EvictionReason reason, object? state)
        {
            if (reason == EvictionReason.Replaced)
                return;
            ClearKeys();
            TryRemoveKey(key.ToString()!);
        }

        public virtual T Get<T>(string key, Func<T> acquire, int? cacheTime = null)
        {
            if (_cache.TryGetValue(key, out T? value))
                return value!;

            var result = acquire();
            if ((cacheTime ?? OSSDefaults.CacheTime) > 0)
                Set(key, result!, cacheTime ?? OSSDefaults.CacheTime);

            return result;
        }

        public virtual void Set(string key, object data, int cacheTime)
        {
            if (data is not null)
                _cache.Set(AddKey(key), data, GetMemoryCacheEntryOptions(TimeSpan.FromMinutes(cacheTime)));
        }

        public virtual bool IsSet(string key)
            => _cache.TryGetValue(key, out object? _);

        public bool PerformActionWithLock(string key, TimeSpan expirationTime, Action action)
        {
            if (!_allKeys.TryAdd(key, true))
                return false;
            try
            {
                _cache.Set(key, key, GetMemoryCacheEntryOptions(expirationTime));
                action();
                return true;
            }
            finally
            {
                Remove(key);
            }
        }

        public virtual void Remove(string key)
            => _cache.Remove(RemoveKey(key));

        public virtual void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matchingKeys = _allKeys.Where(p => p.Value).Select(p => p.Key)
                                       .Where(key => regex.IsMatch(key)).ToList();

            foreach (var key in matchingKeys)
                _cache.Remove(RemoveKey(key));
        }

        public virtual void Clear()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public virtual void Dispose() { }
    }
}

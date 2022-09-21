using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace SiteIndexer.Services.System
{
    public interface ICacheService
    {
        bool IsInCache(string key);
        void Set(string cacheKey, object o, DateTime expiration);
        void Set(string cacheKey, object o, CacheDependency dependency, DateTime expiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback callback);
        T Get<T>(string key);
        void Clear(string key);
    }

    public class CacheService : ICacheService
    {
        #region Constructor

        protected static Cache CacheContext => HttpRuntime.Cache;
        protected readonly ILogService LogService;

        public CacheService(ILogService logService)
        {
            LogService = logService;
        }

        #endregion

        #region Core Methods

        public bool IsInCache(string key)
        {
            var isInCache = CacheContext[key] != null;

            LogService.Debug($"CacheService.IsInCache - cacheKey: {key} - {isInCache}");

            return isInCache;
        }

        public void Set(string cacheKey, object o, DateTime expiration)
        {
            Set(cacheKey, o, null, expiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        }

        public void Set(string cacheKey, object o, CacheDependency dependency, DateTime expiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback callback)
        {
            CacheContext.Add(cacheKey, o, dependency, expiration, slidingExpiration, priority, callback);
        }

        public T Get<T>(string key)
        {
            object o = CacheContext[key];
            var keyExists = string.IsNullOrEmpty(key) == false;
            var typeIsCorrect = o is T;

            if(keyExists && !typeIsCorrect)
                LogService.Error($"Found key: {key} but the type: {typeof(T)} didn't match object");

            return keyExists && typeIsCorrect
                ? (T)o
                : default(T);
        }

        public void Clear(string key)
        {
            LogService.Debug($"CacheService.Clear - cacheKey: {key}");

            CacheContext.Remove(key);
        }

        private void ClearKeys(string keyType)
        {
            if (!IsInCache(keyType))
                return;

            var list = Get<List<string>>(keyType);
            foreach (var key in list)
                Clear(key);

            Clear(keyType);
        }

        private void AddKey(string keyType, string key)
        {
            var list = IsInCache(keyType)
                ? Get<List<string>>(keyType)
                : new List<string>();

            if (list.Contains(key))
                return;

            list.Add(key);
            Set(keyType, list, DateTime.UtcNow.AddHours(1));
        }

        #endregion

        #region Local Methods

        //private string AssignmentByIdToken = "AssignmentByIdKeys";
        //public void ClearAssignmentByIdCache() => ClearKeys(AssignmentByIdToken);
        //public string AssignmentByIdCacheKey(long assignmentId)
        //{
        //    var key = $"AssignmentById-{assignmentId}";
        //    AddKey(AssignmentByIdToken, key);

        //    return key;
        //}

        #endregion
    }
}
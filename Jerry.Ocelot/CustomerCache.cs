using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ocelot.Cache;

namespace Jerry.Ocelot
{
    public class CustomerCache:IOcelotCache<CachedResponse>
    {
        private class CacheDataModel
        {
            public CachedResponse CachedResponse { get; set; }
            public DateTime ExpireTime { get; set; }
            public string Region { get; set; }
        }
        
        private static Dictionary<string, CacheDataModel> _CacheDataModels = new Dictionary<string, CacheDataModel>();

        public void Add(string key, CachedResponse value, TimeSpan ttl, string region)
        {
            if (!_CacheDataModels.ContainsKey($"{region}_{key}"))
            {
                _CacheDataModels.Add($"{region}_{key}", new CacheDataModel()
                {
                    ExpireTime = DateTime.Now.Add(ttl),
                    Region = region,
                    CachedResponse = value
                });
            }
        }

        public CachedResponse Get(string key, string region)
        {
            if (!_CacheDataModels.ContainsKey($"{region}_{key}")) return null;

            var CacheDataModel = _CacheDataModels[$"{region}_{key}"];
            if (CacheDataModel != null && CacheDataModel.ExpireTime >= DateTime.Now)
            {
                return CacheDataModel.CachedResponse;
            }

            _CacheDataModels.Remove($"{region}_{key}");
            return null;

        }

        public void ClearRegion(string region)
        {
            var keysToRemove = _CacheDataModels.Where(c => c.Key.StartsWith($"{region}_"))
                .Select(c => c.Key)
                .ToList();
            foreach (var key in keysToRemove)
            {
                _CacheDataModels.Remove(key);
            }

        }

        public void AddAndDelete(string key, CachedResponse value, TimeSpan ttl, string region)
        {
            if (_CacheDataModels.ContainsKey($"{region}_{key}"))
            {
                _CacheDataModels.Remove($"{region}_{key}");
            }

            _CacheDataModels.Add($"{region}_{key}", new CacheDataModel()
            {
                ExpireTime = DateTime.Now.Add(ttl),
                CachedResponse = value
            });
        }
    }
}

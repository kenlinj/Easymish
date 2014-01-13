using System;
using System.Web.Helpers;

namespace Easymish.Web
{
    public static class Caches
    {
        #region private helper function to get and set cache values

        private static TType GetCache<TType>(Keys key, TType defaultValue)
        {
            string cacheKey = key.GetHashCode().ToString();

            if (WebCache.Get(cacheKey) == null)
            {
                return defaultValue;
            }
            else
            {
                var underlyingType = Nullable.GetUnderlyingType(typeof(TType));

                WebCache.Set(cacheKey, defaultValue, 20, true);

                return (TType)Convert.ChangeType(WebCache.Get(cacheKey), underlyingType ?? typeof(TType));
            }
        }

        #endregion

        public enum Keys
        {
            CompanyUrl,
            CompanyName
        }

        public static string CompanyUrl
        {
            get
            {
                return GetCache(Keys.CompanyUrl, ServiceFactory.Settings.CompanyUrl);
            }
        }

        public static string CompanyName
        {
            get
            {
                return GetCache(Keys.CompanyName, ServiceFactory.Settings.CompanyName);
            }
        }
    }
}

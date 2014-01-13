using System;
using System.Web;

namespace Easymish.Web
{
    public static class ContextItem
    {
        #region private helper function to get and set ContextItem values

        private static TType GetContextItem<TType>(Keys key)
        {
            return GetContextItem(key, default(TType));
        }

        private static TType GetContextItem<TType>(Keys key, TType defaultValue)
        {
            if (HttpContext.Current.Items[key.GetHashCode().ToString()] == null)
            {
                return defaultValue;
            }
            else
            {
                var underlyingType = Nullable.GetUnderlyingType(typeof(TType));

                SetContextItem<TType>(key, defaultValue);

                return (TType)Convert.ChangeType(HttpContext.Current.Items[key.GetHashCode().ToString()], underlyingType ?? typeof(TType));
            }
        }

        private static void SetContextItem<TType>(Keys key, TType value)
        {
            HttpContext.Current.Items[key.GetHashCode().ToString()] = value;
        }

        #endregion

        #region public ContextItem value
        
        public enum Keys
        {
        }


        #endregion
    }
}

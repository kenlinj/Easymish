using System;
using System.Web;

namespace Easymish.Web
{
    public static class Sessions
    {
        #region private helper function to get and set session values

        private static TType GetSession<TType>(Keys key)
        {
            return GetSession(key, default(TType));
        }

        private static TType GetSession<TType>(Keys key, TType defaultValue)
        {
            if (HttpContext.Current.Session[key.GetHashCode().ToString()] == null)
            {
                return defaultValue;
            }
            else
            {
                var underlyingType = Nullable.GetUnderlyingType(typeof(TType));

                SetSession<TType>(key, defaultValue);

                return (TType)Convert.ChangeType(HttpContext.Current.Session[key.GetHashCode().ToString()], underlyingType ?? typeof(TType));
            }
        }

        private static void SetSession<TType>(Keys key, TType sessionValue)
        {
            HttpContext.Current.Session[key.GetHashCode().ToString()] = sessionValue;
        }

        #endregion

        #region public session value
        
        public enum Keys
        {
            LoginName,
            FirstName,
            LastName
        }
        
        public static string SessionID
        {
            get
            {
                return HttpContext.Current.Session.SessionID;
            }
        }

        public static string LoginName
        {
            get { return GetSession<string>(Keys.LoginName); }
            set { SetSession(Keys.LoginName, value); }
        }

        public static string FirstName
        {
            get { return GetSession<string>(Keys.FirstName); }
            set { SetSession(Keys.FirstName, value); }
        }

        public static string LastName
        {
            get { return GetSession<string>(Keys.LastName); }
            set { SetSession(Keys.LastName, value); }
        }

        public static string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }
        public static bool IsAuthenticate
        {
            get { return !string.IsNullOrEmpty(LoginName); }
        }

        #endregion
    }
}

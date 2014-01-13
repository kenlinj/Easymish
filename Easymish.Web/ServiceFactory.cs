using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Easymish.Web
{
    public static class ServiceFactory
    {
        #region private
        
        static IUnityContainer _UnityContainer;

        static ServiceFactory()
        {
            if (_UnityContainer == null)
            {
                _UnityContainer = new UnityContainer();
                UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
                section.Configure(_UnityContainer, "");
            }
        }

        static TServiceType Create<TServiceType>()
        {
            return _UnityContainer.Resolve<TServiceType>();
        }

        #endregion        

        public static ServiceInterface.ISettings Settings { get { return Create<ServiceInterface.ISettings>(); } }
    }
}

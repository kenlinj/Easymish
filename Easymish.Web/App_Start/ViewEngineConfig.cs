using System.Web;
using System.Web.Mvc;

namespace Easymish.Web
{
    public class ViewEngineConfig
    {
        public static void RegisterViewEngine(ViewEngineCollection viewEngines)
        {
            viewEngines.Add(new ViewEngine.PartialViewEngine()); 
        }
    }
}
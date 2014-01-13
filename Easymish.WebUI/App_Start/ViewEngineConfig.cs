using System.Web;
using System.Web.Mvc;

namespace Easymish.WebUI
{
    public class ViewEngineConfig
    {
        public static void RegisterViewEngine(ViewEngineCollection viewEngines)
        {
            viewEngines.Add(new Web.ViewEngine.PartialViewEngine()); 
        }
    }
}
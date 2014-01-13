using System.Linq;
using System.Web.Mvc;

namespace Easymish.Web.ViewEngine
{
    public  class PartialViewEngine : RazorViewEngine
    {
        private static string[] NewPartialViewFormats = new[] { 
            "~/Views/{1}/Partials/{0}.cshtml",
            "~/Views/Shared/Partials/{0}.cshtml"
        };

        public PartialViewEngine()
        {
            base.PartialViewLocationFormats = base.PartialViewLocationFormats.Union(NewPartialViewFormats).ToArray();
        }
    }
}

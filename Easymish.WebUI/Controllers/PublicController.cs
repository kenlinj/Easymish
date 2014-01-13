using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Easymish.WebUI.Controllers
{
    public class PublicController : Controller
    {
        public PublicController()
        {
        }
        public ActionResult Index()
        {
            if(Web.Sessions.IsAuthenticate)
            {
                return RedirectToAction("", "home");
            }

            return View();
        }

    }
}

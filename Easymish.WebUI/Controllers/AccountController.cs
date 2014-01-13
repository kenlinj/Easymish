using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;

namespace Easymish.WebUI.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            Models.AccountLogin model = new Models.AccountLogin();
            XmlDocument doc = new XmlDocument();
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(Models.AccountLogin model)
        {
            //if (ModelState.IsValid)
            //{
            //    ModelState.Remove("HasError");

            //    Entity.User user = Business.Users.GetUser(model.Username, model.Password);
            //    if (user == null)
            //    {
            //        model.HasError = true;
            //    }
            //    else
            //    {
            //        AccessControl.InitializeLoginInfo(user);
            //        return RedirectToAction("", "home");
            //    }
            //}

            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}

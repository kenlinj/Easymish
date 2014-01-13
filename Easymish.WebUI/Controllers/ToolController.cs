using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Text;

namespace Easymish.WebUI.Controllers
{
    public class ToolController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RegularExpressionReplacement()
        {
            return View(new Models.RegularExpressionReplacement());
        }

        [HttpPost]
        public ActionResult RegularExpressionReplacement(Models.RegularExpressionReplacement model)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                MatchCollection matches = Regex.Matches(model.InputText, model.RegularExpression);

                foreach (Match match in matches)
                {
                    sb.AppendLine(match.Value);
                }
            }
            catch
            {
            }

            model.ResultText = sb.ToString();

            return View(model);
        }

    }
}

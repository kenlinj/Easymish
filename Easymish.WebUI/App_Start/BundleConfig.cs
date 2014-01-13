using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Optimization;
using System.Xml;

namespace Easymish.WebUI
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            //System.Web.Routing.
            //AddMVCBundle("users", "index");
            RegisterPageBundles(bundles);
            RegisterIndividualBundles(bundles);
            RegisterOtherBundles(bundles);
            //BundleTable.EnableOptimizations = true;
        }

        static void RegisterPageBundles(BundleCollection bundles)
        {
            string[] controllerFolders = Directory.GetDirectories(HttpContext.Current.Server.MapPath("~/views"));

            string[] excludeControllers = new string[]{
                "shared"
            };

            foreach (string controllerFolder in controllerFolders)
            {
                string controller = Path.GetFileNameWithoutExtension(controllerFolder);

                if (!excludeControllers.Any(excludeControl => excludeControl == controller))
                {
                    string[] actionFiles = Directory.GetFiles(controllerFolder, "*.cshtml");

                    foreach (string actionFile in actionFiles)
                    {
                        string action = Path.GetFileNameWithoutExtension(actionFile);
                        AddPageBundle(bundles, controller, action, true);
                        AddPageBundle(bundles, controller, action, false);
                    }
                }
            }
        }

        static void RegisterIndividualBundles(BundleCollection bundles)
        {
            AddIndividualBundle(bundles, true);
            AddIndividualBundle(bundles, false);
        }

        static void AddPageBundle(BundleCollection bundles, string controller, string action, bool javascript)
        {
            string scriptPath;

            #region get script path
            if (javascript)
            {
                scriptPath = string.Format("~/js/page/{0}/{1}.js", controller.ToLower(), action.ToLower());
            }
            else
            {
                scriptPath = string.Format("~/css/page/{0}/{1}.css", controller.ToLower(), action.ToLower());
            }
            
            #endregion   
            
            string scriptPhysicalPath = HttpContext.Current.Server.MapPath(scriptPath);

            if (File.Exists(scriptPhysicalPath))
            {
                string bundlePath;

                #region create bundle path: ~/bundles/js/[controller]/[action] or ~/bundles/css/[controller]/[action]
                if (javascript)
                {
                    bundlePath = string.Format("~/bundles/js/{0}/{1}", controller.ToLower(), action.ToLower());
                }
                else
                {
                    bundlePath = string.Format("~/bundles/css/{0}/{1}", controller.ToLower(), action.ToLower());
                }
                
                #endregion

                AddBundle(bundles, bundlePath, scriptPath);
            }
        }

        static void AddIndividualBundle(BundleCollection bundles, bool javascript)
        {
            string physicalRootPath;
            string rootPath;
            string bundleRootPath;
            string ext;

            if (javascript)
            {
                rootPath = "~/js";
                ext = "js";
                bundleRootPath = "~/bundles/js";
                physicalRootPath = HttpContext.Current.Server.MapPath("~/js");
            }
            else
            {
                rootPath = "~/css";
                ext = "css";
                bundleRootPath = "~/bundles/css";
                physicalRootPath = HttpContext.Current.Server.MapPath("~/css");
            }

            string[] files = Directory.GetFiles(physicalRootPath, "*." +  ext, SearchOption.AllDirectories);
            string[] excludeFolders = new string[]{
                "page", 
                "images"
            };

            foreach (string file in files)
            {
                string relativePath = file.Remove(0, physicalRootPath.Length);
                relativePath = relativePath.Replace("\\", "/");

                if (!excludeFolders.Any(folder=>relativePath.StartsWith(string.Format("/{0}/", folder))))
                {
                    string bundlePath = relativePath;
                    
                    if (javascript)
                    {
                        //remove version number at the end
                        bundlePath = Regex.Replace(bundlePath, @"-(\d+(?:.\d+){1,3})[.]js", "");
                        bundlePath = Regex.Replace(bundlePath, @"[.]js", "");
                    }
                    else 
                    {
                        bundlePath = relativePath.Remove(relativePath.LastIndexOf("."));
                    }
                    
                    bundlePath = bundleRootPath + bundlePath;
                    relativePath = rootPath + relativePath;

                    if (javascript)
                    {
                        AddBundle(bundles, bundlePath, relativePath);
                    }
                    else
                    {
                        AddBundle(bundles, bundlePath, relativePath);
                    }
                }
            }
        }

        static void RegisterOtherBundles(BundleCollection bundles)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            Stream stream = assembly.GetManifestResourceStream("Easymish.WebUI.bundles.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            XmlNodeList bundleNodes = doc.SelectNodes("bundles/bundle/@name");

            foreach (XmlNode bundleNode in bundleNodes)
            {
                XmlNodeList bundleScriptNodes = doc.SelectNodes(string.Format("bundles/bundle[@name='{0}']/script", bundleNode.Value));

                foreach (XmlNode bundleScriptNode in bundleScriptNodes)
                {
                    AddBundle(bundles, bundleNode.Value, bundleScriptNode.InnerText);
                }
            }      
        }

        static void AddBundle(BundleCollection bundles, string bundlePath, string scriptPath)
        {
            bool isJavascript = scriptPath.EndsWith(".js");

            if (bundles.Any(b => b.Path == bundlePath))
            {
                bundles.First(b => b.Path == bundlePath).Include(scriptPath);
            }
            else
            {
                if (isJavascript)
                {
                    bundles.Add(new ScriptBundle(bundlePath).Include(scriptPath));
                }
                else
                {
                    bundles.Add(new StyleBundle(bundlePath).Include(scriptPath, new CssRewriteUrlTransform()));
                }
            }
        }
    }
}

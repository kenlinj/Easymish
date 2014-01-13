using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Optimization;

namespace Easymish.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            //get bundle info from "bundles.xml" defined in config file. Register bundle with the given name and scripts.
            RegisterBundlesFromConfigFile(bundles);

            //register bundle with control name and action name. the control name and action name comes from View folder name and file name.
            //bundle name is ~/bundles/js|css/[controller]/[action]. script path must be ~/js|css/page/[controller]/[action]. If script path doesn't exist, don't add the bundle.
            RegisterPageBundles(bundles);

            //Register all the scripts in case they are not include in the previous steps. Normally all the scripts must include in previous steps.
            //bundle name is ~/bundles/js|css/[script name without extension]. it includes version number already.
            RegisterIndividualBundles(bundles);

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

        static void RegisterBundlesFromConfigFile(BundleCollection bundles)
        {
            ConfigurationSections.BundlesConfigurationSection bundlesSection = (ConfigurationSections.BundlesConfigurationSection)System.Configuration.ConfigurationManager.GetSection("bundles");

            foreach (var objBundleSection in bundlesSection.Bundles)
            {
                var bundleSection = (ConfigurationSections.BundlesConfigurationSection.BundleElement)objBundleSection;

                foreach (var objScriptSection in bundleSection.Scripts)
                {
                    var scriptSection = (ConfigurationSections.BundlesConfigurationSection.ScriptElement)objScriptSection;
                    AddBundle(bundles, bundleSection.Name, scriptSection.Value);
                }
            }
        }

        static void AddBundle(BundleCollection bundles, string bundlePath, string scriptPath)
        {
            bool isJavascript = scriptPath.EndsWith(".js");
            var resolver = new BundleResolver(bundles);
                
            if (bundles.Any(b => b.Path == bundlePath))
            {
                var bundle = bundles.First(b => b.Path == bundlePath);
                var contents = resolver.GetBundleContents(bundle.Path);

                if (!contents.Any(s => s == scriptPath))
                {
                    //if bundle already exists, include the script path
                    bundle.Include(scriptPath);
                }
            }
            else
            {
                //if bundle not exists, create the bundle and include the script path
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

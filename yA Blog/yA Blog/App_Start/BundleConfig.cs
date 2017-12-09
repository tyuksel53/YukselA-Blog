using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
namespace yA_Blog.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new StyleBundle("~/css/all").Include(
                "~/Content/css/bootstrap.min.css",
                "~/Content/css/font-awesome.min.css",
                "~/Content/css/main.min.css"
                ));

                 bundles.Add(new ScriptBundle("~/script/all").Include(
                "~/Scripts/vendor/modernizr.custom.32229-2.8-respondjs-1-4-2.js",
                "~/Scripts/jquery-3.2.1.min.js",
                "~/Scripts/vendor/jquery.jpanelmenu.min.js",
                "~/Scripts/umd/popper.min.js",
                "~/Scripts/vendor/bootstrap.min.js",
                "~/Scripts/vendor/fastclick.min.js",
                "~/Scripts/main.js"
                ));

            bundles.Add(new ScriptBundle("~/script/validate").Include(
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js"
                ));

            bundles.Add(new ScriptBundle("~/script/ajax").Include(
                "~/Scripts/jquery.unobtrusive-ajax.min.js"
                ));

            BundleTable.EnableOptimizations = true;
        }
    }
}
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
                "~/Content/bootstrap.min.css",
                "~/Content/Site.css"
                ));
            bundles.Add(new ScriptBundle("~/script/all").Include(
                "~/Scripts/jquery-3.2.1.slim.js",
                "~/Scripts/jquery-3.2.1.min.js",
                "~/Scripts/umd/popper.min.js",
                "~/Scripts/bootstrap.min.js",
                "~/Scripts/modernizr-2.8.3.js"
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
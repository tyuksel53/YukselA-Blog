using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using yA_Blog.Areas.Blog.Models;

namespace yA_Blog.Areas.Blog.Filter
{
    public class AuthAdmin:FilterAttribute,IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["Kullanici"] == null)
            {
                filterContext.Result = new RedirectResult("/Home/Index");
            }
            else
            {
                Kullanici admin = filterContext.HttpContext.Session["Kullanici"] as Kullanici;
                if (!admin.Role.Equals("admin"))
                {
                    filterContext.Result = new RedirectResult("/Home/Index");
                }
            }
        }
    }
}
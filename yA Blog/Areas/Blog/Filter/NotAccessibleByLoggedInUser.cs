using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace yA_Blog.Areas.Blog.Filter
{
    public class NotAccessibleByLoggedInUser : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["Kullanici"] != null)
            {
                filterContext.Result = new RedirectResult("/Blog/Home/Index");
            }
        }
    }
}
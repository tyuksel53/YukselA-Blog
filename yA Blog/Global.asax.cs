using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using yA_Blog.App_Start;
using yA_Blog.Areas.Blog.Models;
using yA_Blog.Areas.Blog.Models.Managers;

namespace yA_Blog
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_Start()
        {
            Session["_loginTryCount"] = 0;
            Session["_userCreateTryCount"] = 0;
            Session["_forgetPasswordTryCount"] = 0;
            Session["_takipciDeneme"] = 0;
            Session["_yorumSayisi"] = 0;

            if (HttpContext.Current.Session["Kullanici"] == null && HttpContext.Current.Request.Cookies.Get("acct") != null)
            {
                var cookie = HttpContext.Current.Request.Cookies.Get("acct");
                if (cookie.Value.Contains("Username=") && cookie.Value.Contains("&Password="))
                {
                    int usernameStart = "Username=".Length;
                    int usernameEnd = cookie.Value.IndexOf("&Password=", StringComparison.Ordinal);
                    string username = cookie.Value.Substring(usernameStart, (usernameEnd - usernameStart));

                    int passStart = usernameEnd + "Password=".Length + 1;
                    string passsword = cookie.Value.Substring(passStart);

                    DatabaseContext db = new DatabaseContext();
                    Kullanici check = db.Kullanicilar.FirstOrDefault(x => x.KullaniciAdi == username);

                    if (check != null)
                    {

                        if (check.Parola.Equals(passsword))
                        {
                            HttpContext.Current.Session["Kullanici"] = check;
                        }
                        else
                        {
                            cookie.Expires = DateTime.Now.AddDays(-1);
                            HttpContext.Current.Response.Cookies.Add(cookie);
                        }

                    }
                    else
                    {
                        cookie.Expires = DateTime.Now.AddDays(-1);
                        HttpContext.Current.Request.Cookies.Add(cookie);
                    }
                }
            }

        }
    }
}

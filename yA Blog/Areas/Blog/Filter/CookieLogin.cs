using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using yA_Blog.Areas.Blog.Models;
using yA_Blog.Areas.Blog.Models.Managers;

namespace yA_Blog.Areas.Blog.Filter
{
    public class CookieLogin : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (HttpContext.Current.Session["Kullanici"] == null && filterContext.HttpContext.Request.Cookies.Get("acct") != null)
            {
                var cookie = filterContext.HttpContext.Request.Cookies.Get("acct");
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
                            filterContext.HttpContext.Response.Cookies.Add(cookie);
                        }

                    }
                    else
                    {
                        cookie.Expires = DateTime.Now.AddDays(-1);
                        filterContext.HttpContext.Response.Cookies.Add(cookie);
                    }
                }
            }
        }
    }
}
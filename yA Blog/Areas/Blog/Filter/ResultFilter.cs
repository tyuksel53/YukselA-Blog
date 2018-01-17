using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using yA_Blog.Areas.Blog.Models;
using yA_Blog.Areas.Blog.Models.Managers;

namespace yA_Blog.Areas.Blog.Filter
{
    public class ResultFilter:FilterAttribute,IResultFilter
    {
        DatabaseContext _dbContext = new DatabaseContext();

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var user = filterContext.HttpContext.Session["Kullanici"] as Kullanici;
            var userIp = filterContext.HttpContext.Request.UserHostAddress;
            _dbContext.Loglar.Add(new Loglar()
            {
                KullaniciAdi = user.KullaniciAdi,
                KullaniciIp = userIp,
                Tarih = DateTime.Now,
                ActionName = filterContext.RouteData.Values["Action"].ToString(),
                ControllerName = filterContext.RouteData.Values["Controller"].ToString(),
                Bilgi = "OnResultExecuting"
            });

            _dbContext.SaveChanges();
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var user = filterContext.HttpContext.Session["Kullanici"] as Kullanici;
            var userIp = filterContext.HttpContext.Request.UserHostAddress;
            _dbContext.Loglar.Add(new Loglar()
            {
                KullaniciAdi = user.KullaniciAdi,
                KullaniciIp = userIp,
                Tarih = DateTime.Now,
                ActionName = filterContext.RouteData.Values["Action"].ToString(),
                ControllerName = filterContext.RouteData.Values["Controller"].ToString(),
                Bilgi = "OnResultExecuted"
            });

            _dbContext.SaveChanges();
        }
    }
}
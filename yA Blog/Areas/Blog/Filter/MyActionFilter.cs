using System;
using System.Web.Mvc;
using yA_Blog.Areas.Blog.Models;
using yA_Blog.Areas.Blog.Models.Managers;

namespace yA_Blog.Areas.Blog.Filter
{
    public class MyActionFilter:FilterAttribute,IActionFilter
    {
        DatabaseContext _dbContext =  new DatabaseContext();

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = filterContext.HttpContext.Session["Kullanici"] as Kullanici;
            var userIp = filterContext.HttpContext.Request.UserHostAddress;
            _dbContext.Loglar.Add(new Loglar()
            {
                KullaniciAdi = user.KullaniciAdi,
                KullaniciIp = userIp,
                Tarih = DateTime.Now,
                ActionName = filterContext.ActionDescriptor.ActionName,
                ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                Bilgi = "OnActionExecuting"
            });

            _dbContext.SaveChanges();
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var user = filterContext.HttpContext.Session["Kullanici"] as Kullanici;

            _dbContext.Loglar.Add(new Loglar()
            {
                KullaniciAdi = user.KullaniciAdi,
                KullaniciIp = filterContext.HttpContext.Request.UserHostAddress,
                Tarih = DateTime.Now,
                ActionName = filterContext.ActionDescriptor.ActionName,
                ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                Bilgi = "OnActionExecuted"
            });

            _dbContext.SaveChanges();
        }
    }
}
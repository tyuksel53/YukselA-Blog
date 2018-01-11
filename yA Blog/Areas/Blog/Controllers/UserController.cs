using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using yA_Blog.Areas.Blog.Filter;
using yA_Blog.Areas.Blog.Models;
using yA_Blog.Areas.Blog.Models.Managers;
using yA_Blog.Areas.Blog.PageModels;

namespace yA_Blog.Areas.Blog.Controllers
{
    [CookieLogin]
    [AuthUser]
    public class UserController : Controller
    {
        readonly DatabaseContext _dB = new DatabaseContext();

       
        [HttpGet]
        public ActionResult Profil()
        {
            
            return View();
        }

        [HttpGet]
        public ActionResult SifreDegis()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SifreDegis(KullaniciGuncelleme updateInfo)
        {
            if (!String.IsNullOrWhiteSpace(updateInfo.EskiParola) && !String.IsNullOrWhiteSpace(updateInfo.YeniParola) && !String.IsNullOrWhiteSpace(updateInfo.YeniParolaOnay))
            {
                if (!updateInfo.YeniParola.Equals(updateInfo.YeniParolaOnay))
                {
                    return View(new KullaniciGuncelleme());
                }

                var user = Session["Kullanici"] as Kullanici;

                if (Crypto.VerifyHashedPassword(user.Parola, updateInfo.EskiParola))
                {
                    Kullanici updateUser = _dB.Kullanicilar.FirstOrDefault(x => x.KullaniciAdi == user.KullaniciAdi);

                    updateUser.Parola = Crypto.HashPassword(updateInfo.YeniParola);

                    _dB.SaveChanges();
                    Session.Clear();

                    var cookie = HttpContext.Request.Cookies.Get("acct");
                    cookie.Value = $"Username={updateUser.KullaniciAdi}&Password={updateUser.Parola}";
                    HttpContext.Response.Cookies.Add(cookie);

                    Session["Kullanici"] = updateUser;

                    ViewBag.Success = "Basarili";
                    
                }
                else
                {
                    ModelState.AddModelError("","Guncel sifreinizi yanlis girdiniz");
                }
            }

            updateInfo.EskiParola = "";
            updateInfo.YeniParola = "";
            updateInfo.YeniParolaOnay = "";

            return View(updateInfo);
        }

        [HttpGet]
        public ActionResult EpostaDegis()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EpostaDegis(KullaniciGuncelleme updateInfo)
        {
            if (!String.IsNullOrWhiteSpace(updateInfo.EskiParola) && !String.IsNullOrWhiteSpace(updateInfo.Email) && !String.IsNullOrWhiteSpace(updateInfo.EmailOnay))
            {
                if (!updateInfo.Email.Equals(updateInfo.EmailOnay))
                {
                    return View(updateInfo);
                }

                var user = Session["Kullanici"] as Kullanici;

                if (Crypto.VerifyHashedPassword(user.Parola, updateInfo.EskiParola))
                {
                    Kullanici updateUser = _dB.Kullanicilar.FirstOrDefault(x => x.KullaniciAdi == user.KullaniciAdi);

                    updateUser.Eposta = updateInfo.Email;

                    _dB.SaveChanges();
                    Session.Clear();

                    var cookie = HttpContext.Request.Cookies.Get("acct");
                    cookie.Value = $"Username={updateUser.KullaniciAdi}&Password={updateUser.Parola}";
                    HttpContext.Response.Cookies.Add(cookie);

                    Session["Kullanici"] = updateUser;

                    ViewBag.Success = "Basarili";

                }
                else
                {
                    ModelState.AddModelError("", "Guncel sifreinizi yanlis girdiniz");
                }
            }

            updateInfo.EskiParola = "";

            return View(updateInfo);
        }
    }
}
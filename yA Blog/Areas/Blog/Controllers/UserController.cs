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
    public class UserController : Controller
    {
        readonly DatabaseContext _dB = new DatabaseContext();

        [CookieLogin]
        [AuthUser]
        [HttpGet]
        public ActionResult Profil(string username)
        {
            var user = Session["Kullanici"] as Kullanici;
            KullaniciGuncelleme update = new KullaniciGuncelleme()
            {
                Email = user.Eposta,
                EmailOnay = user.Eposta
            };
            
            return View(update);
        }

        [HttpPost]
        public ActionResult Profil(KullaniciGuncelleme updateInfo)
        {
            if (ModelState.IsValid)
            {
                var user = Session["Kullanici"] as Kullanici;

                if (Crypto.VerifyHashedPassword(user.Parola, updateInfo.EskiParola))
                {
                    Kullanici updateUser = _dB.Kullanicilar.FirstOrDefault(x => x.KullaniciAdi == user.KullaniciAdi);

                    updateUser.Parola = Crypto.HashPassword(updateInfo.YeniParola);
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
                    ModelState.AddModelError("","Guncel sifreinizi yanlis girdiniz");
                }
            }
            updateInfo.EskiParola = "";
            updateInfo.YeniParola = "";
            updateInfo.YeniParolaOnay = "";

            return View(updateInfo);
        }
    }
}
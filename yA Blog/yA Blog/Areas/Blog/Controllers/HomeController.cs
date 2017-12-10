using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using yA_Blog.Areas.Blog.Models;
using yA_Blog.Areas.Blog.Models.Managers;
using yA_Blog.Library;

namespace yA_Blog.Areas.Blog.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GirisYap()
        {
            return View(new Kullanici());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login_Check(string KullaniciAdi, string Parola)
        {
            bool isLogin = false;

            var response = Request["g-recaptcha-response"];

            const string secret = "6LcncjwUAAAAACSXIynAx_42X0UteOk0VeXkPBVY";
            
            var client = new WebClient();
            var reply =
                client.DownloadString(
                    string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

            if (!captchaResponse.Success)
            {
                return Json("captchaError", JsonRequestBehavior.AllowGet);
            }

            DatabaseContext db = new DatabaseContext();
            Kullanici check = (from s in db.Kullanicilar where (s.KullaniciAdi == KullaniciAdi && s.Parola == Parola) select s ).FirstOrDefault();
           
            if (check != null)
            {
                System.Threading.Thread.Sleep(1000);
                Session["Kullanici"] = check;
                HttpCookie acct = new HttpCookie("acct", check.KullaniciAdi + check.Parola)
                {
                    Expires = DateTime.Now.AddMonths(1)
                };
                isLogin = true;
                HttpContext.Response.Cookies.Add(acct);
            }

            return Json(isLogin, JsonRequestBehavior.AllowGet);
        }
    }
}
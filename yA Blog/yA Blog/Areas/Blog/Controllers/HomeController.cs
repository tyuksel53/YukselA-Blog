using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using yA_Blog.Areas.Blog.Models;
using yA_Blog.Areas.Blog.Models.Managers;
using yA_Blog.Filters;
using yA_Blog.Library;

namespace yA_Blog.Areas.Blog.Controllers
{
    public class HomeController : Controller
    {
        DatabaseContext db = new DatabaseContext();

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

        [HttpGet]
        public ActionResult Yeni_Kayit()
        {
            ViewBag.recaptchaKey =  WebConfigurationManager.AppSettings["recaptcha_sitekey"].ToString();
            return View(new Kullanici());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Yeni_Kayit(Kullanici model)
        {
            ViewBag.recaptchaKey = WebConfigurationManager.AppSettings["recaptcha_sitekey"].ToString();
            var response = Request["g-recaptcha-response"];
            bool captcha_check = CheckCaptcha(response: response);

            if (!captcha_check)
            {
                ViewBag.captcha_error = "Lütfen güvenligi dogrulayınız";
                return View(model);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    Kullanici yeniUser = (from s in db.Kullanicilar where s.KullaniciAdi == model.KullaniciAdi select s).FirstOrDefault();
                    if (yeniUser != null)
                    {
                        ModelState.AddModelError("", "Girdiginiz kullanici adi kullanılmaktadır");
                        return View(model);
                    }
                    else
                    {
                        db.Kullanicilar.Add(model);
                        db.SaveChanges();
                        Session["Kullanici"] = model;
                        HttpCookie acct = new HttpCookie("acct", model.KullaniciAdi + model.Parola)
                        {
                            Expires = DateTime.Now.AddMonths(1)
                        };
                        HttpContext.Response.Cookies.Add(acct);

                        return RedirectToAction("Index");
                    }
                }else
                {
                    return View(model);
                }

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login_Check(string KullaniciAdi, string Parola)
        {
            bool isLogin = false;

            var response = Request["g-recaptcha-response"];
            bool captcha_check = CheckCaptcha(response: response);

            if(!captcha_check)
            {
                Json("captchaError", JsonRequestBehavior.AllowGet);
            }


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
        public bool CheckCaptcha(string response)
        {

            string secret = WebConfigurationManager.AppSettings["recaptcha_privatekey"].ToString();

            var client = new WebClient();
            var reply =
                client.DownloadString(
                    string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

            if (!captchaResponse.Success)
            {
                return false;

            }else
            {
                return true;
            }
        }
    }
}
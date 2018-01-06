using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using yA_Blog.Areas.Blog.Models;
using yA_Blog.Areas.Blog.Models.Managers;
using yA_Blog.Library;
using System.Web.Helpers;
using yA_Blog.Areas.Blog.Library;

namespace yA_Blog.Areas.Blog.Controllers
{
    public class HomeController : Controller
    {
        readonly DatabaseContext _db = new DatabaseContext();

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
            ViewBag.recaptchaKey =  WebConfigurationManager.AppSettings["recaptcha_sitekey"];
            return View(new Kullanici());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Yeni_Kayit(Kullanici model)
        {
            ViewBag.recaptchaKey = WebConfigurationManager.AppSettings["recaptcha_sitekey"];
            var response = Request["g-recaptcha-response"];
            bool captchaCheck = CheckCaptcha(response: response);

            if (!captchaCheck)
            {
                ViewBag.captcha_error = "Lütfen güvenligi dogrulayınız";
                return View(model);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    Kullanici yeniUser = (from s in _db.Kullanicilar where s.KullaniciAdi == model.KullaniciAdi select s).FirstOrDefault();
                    if (yeniUser != null)
                    {
                        ModelState.AddModelError("", "Girdiginiz kullanici adi kullanılmaktadır");
                        return View(model);
                    }
                    else
                    {
                        model.Parola = Crypto.HashPassword(model.Parola);
                        _db.Kullanicilar.Add(model);
                        _db.SaveChanges();
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
        public ActionResult Login_Check(string kullaniciAdi, string parola)
        {
            bool isLogin = false;
            
            var response = Request["g-recaptcha-response"];
            bool captchaCheck = CheckCaptcha(response: response);

            if(!captchaCheck)
            {
                Json("captchaError", JsonRequestBehavior.AllowGet);
            }

            Kullanici check = (from s in _db.Kullanicilar where (s.KullaniciAdi == kullaniciAdi )  select s ).FirstOrDefault();
           
            if (check != null)
            {
                if(Crypto.VerifyHashedPassword(check.Parola,parola))
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

        [HttpPost]
        public JsonResult takipciKaydet(string Email)
        {
            bool check = Portal.IsValidMail(Email);
            string message = "Lütfen düzgün bir mail adresi girin";

            if (check)
            {
                Takipciler isMailExists = _db.Subscribers.FirstOrDefault(x => x.Email == Email);
                if (isMailExists != null)
                {
                    message = "zaten var";
                }
                else
                {
                    Takipciler yeniTakipci = new Takipciler()
                    {
                        Email = Email,
                        KayitTarihi = DateTime.Now.ToString("dd-MM-yyyy"),
                        DelToken = Guid.NewGuid()
                    };
                    _db.Subscribers.Add(yeniTakipci);
                    _db.SaveChanges();
                    message = "basarili";
                }
               
            }
            return Json(message);
        }
    }
}
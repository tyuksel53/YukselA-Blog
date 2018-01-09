﻿using System;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using yA_Blog.Areas.Blog.Models;
using yA_Blog.Areas.Blog.Models.Managers;
using System.Web.Helpers;
using yA_Blog.Areas.Blog.Library;

namespace yA_Blog.Areas.Blog.Controllers
{
    public class HomeController : Controller
    {
        private DatabaseContext _db = new DatabaseContext();

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
            bool captchaCheck = Portal.CheckCaptcha(response: response);

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
                        model.ActivateGuid = Guid.NewGuid();
                        model.IsActive = false;
                        model.Role = "user";

                        _db.Kullanicilar.Add(model);
                        _db.SaveChanges();

                        Portal.AktivasyonMailGonder(model);

                        return RedirectToAction("UserCreate","Home");
                    }
                }else
                {
                    return View(model);
                }

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GirisYap(string kullaniciAdi, string parola)
        {
            var kullanici = new Kullanici()
            {
                KullaniciAdi = kullaniciAdi,
                Parola = parola
            };
            var response = Request["g-recaptcha-response"];
            bool captchaCheck = Portal.CheckCaptcha(response: response);

            if(!captchaCheck)
            {
                ModelState.AddModelError("","Lütfen günveligi dogrulayınız");
                return View(kullanici);
            }

            Kullanici check = _db.Kullanicilar.FirstOrDefault(x => x.KullaniciAdi == kullaniciAdi);
           
            if (check != null)
            {
                if(Crypto.VerifyHashedPassword(check.Parola,parola))
                {
                    System.Threading.Thread.Sleep(1000);

                    if (check.IsActive)
                    {
                        Session["Kullanici"] = check;
                        HttpCookie acct = new HttpCookie("acct", check.KullaniciAdi + check.Parola)
                        {
                            Expires = DateTime.Now.AddMonths(1)
                        };
                        HttpContext.Response.Cookies.Add(acct);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Hesabınızı aktiflestirmediniz. Hesabınızı aktiflestirmek için size" +
                                                     " tekrar mail gönderdik. Maildeki talimatları uygulayarak hesabınızı" +
                                                     " aktiflestirebilirsiniz.");

                        Portal.AktivasyonMailGonder(check);

                        return View(kullanici);
                    }
                    

                }
            }

            ModelState.AddModelError("", "Yanlıs kullanici adı veya sifre");
            return View(kullanici);


        }

        [HttpPost]
        public JsonResult TakipciKaydet(string Email)
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

        public ActionResult UserCreate()
        {
            ViewBag.ResultMessage =
                "Hesabınız oluşturuldu. Kullanmaya başlamadan önce eposta adresinizi doğrulamalısınız.";

            return View();
        }

        public ActionResult UserActivate(Guid activateId)
        {

            Kullanici active = _db.Kullanicilar.FirstOrDefault(x => x.ActivateGuid == activateId);

            if (active != null)
            {
                if (active.IsActive)
                {
                    ViewBag.ResultMessage = "Kullanıcı zaten aktif edilmiştir.";
                }
                else
                {
                    ViewBag.ResultMessage = "Hesabınız aktifleştirildi.";
                    active.IsActive = true;
                    _db.SaveChanges();
                    //TODO burada login yapacan kullanıcııyı
                }
            }
            else
            {
                ViewBag.ResultMessage = "Aktifleştirilecek kullanıcı bulunamadı !";
            }
            return View();
        }
    }
}
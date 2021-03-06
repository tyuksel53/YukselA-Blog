﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using yA_Blog.Areas.Blog.Models;
using yA_Blog.Areas.Blog.Models.Managers;
using System.Web.Helpers;
using yA_Blog.Areas.Blog.Filter;
using yA_Blog.Areas.Blog.Library;
using yA_Blog.Areas.Blog.PageModels;

namespace yA_Blog.Areas.Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseContext _db = new DatabaseContext();

        [HttpGet]
        public ActionResult Index()
        {
            HomeIndex homePage = new HomeIndex();
            homePage.Haberler = _db.Haberler.Where(x=> x.Taslak == false).OrderByDescending(x=> x.ID).ToList();
            homePage.Kategoriler = CacheHelper.KategoriGet();

            var yorumCount = (from item in _db.Yorumlar
                group item by item.PostId
                into g
                select new {Item = g.Key, Count = g.Count()}).ToList();

            var altYorumCount = (from item in _db.AltYorumlar
                group item by item.PostId
                into g
                select new { Item = g.Key, Count = g.Count() }).ToList();

            if (yorumCount.Count > 0)
            {
                List<Tuple<int, int>> yorumGrup = new List<Tuple<int, int>>();
                for (int i = 0; i < yorumCount.Count; i++)
                {
                    
                    var count = altYorumCount.FirstOrDefault(x => x.Item == yorumCount.ElementAt(i).Item);
                    yorumGrup.Add(count != null
                        ? new Tuple<int, int>(yorumCount.ElementAt(i).Item, yorumCount.ElementAt(i).Count + count.Count)
                        : new Tuple<int, int>(yorumCount.ElementAt(i).Item, yorumCount.ElementAt(i).Count));
                }

                homePage.YorumCount = yorumGrup;
            }

            return View(homePage);
        }

        [NotAccessibleByLoggedInUser]
        [HttpGet]
        public ActionResult GirisYap()
        {
            ViewBag.LoginTryCount = Session["_loginTryCount"];
            ViewBag.recaptchaKey = Portal.WebConfigGet<string>("recaptcha_sitekey");
            return View(new Kullanici());
        }

        [NotAccessibleByLoggedInUser]
        [HttpGet]
        public ActionResult Yeni_Kayit()
        {
            ViewBag.UserTryCount = Session["_userCreateTryCount"];
            ViewBag.recaptchaKey = Portal.WebConfigGet<string>("recaptcha_sitekey");
            return View(new Kullanici());
        }

        [NotAccessibleByLoggedInUser]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Yeni_Kayit(Kullanici model)
        {
            Session["_userCreateTryCount"] = Convert.ToInt32(Session["_userCreateTryCount"]) + 1;
            ViewBag.UserTryCount = Session["_userCreateTryCount"];

            if (Convert.ToInt32(Session["_userCreateTryCount"]) > 5)
            {
                ViewBag.recaptchaKey = Portal.WebConfigGet<string>("recaptcha_sitekey");
                var response = Request["g-recaptcha-response"];
                bool captchaCheck = Portal.CheckCaptcha(response: response);

                if (!captchaCheck)
                {
                    ViewBag.captcha_error = "Lütfen güvenligi dogrulayınız";
                    return View(model);
                }
            }

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

                    Session["_userCreateTryCount"] = 0;

                    return RedirectToAction("UserCreate", "Home");
                }
            }
            else
            {
                return View(model);
            }
        }

        [NotAccessibleByLoggedInUser]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GirisYap(string kullaniciAdi, string parola)
        {
            Session["_loginTryCount"] = Convert.ToInt32(Session["_loginTryCount"]) + 1;
            ViewBag.LoginTryCount = Session["_loginTryCount"];
            ViewBag.recaptchaKey = Portal.WebConfigGet<string>("recaptcha_sitekey");
            var kullanici = new Kullanici()
            {
                KullaniciAdi = kullaniciAdi,
                Parola = parola
            };

            if (Convert.ToInt32(Session["_loginTryCount"]) > 5)
            {
                var response = Request["g-recaptcha-response"];
                bool captchaCheck = Portal.CheckCaptcha(response: response);

                if (!captchaCheck)
                {
                    ModelState.AddModelError("", "Lütfen günveligi dogrulayınız");
                    return View(kullanici);
                }
            }

            Kullanici check = _db.Kullanicilar.FirstOrDefault(x => x.KullaniciAdi == kullaniciAdi);
           
            if (check != null)
            {
                if(Crypto.VerifyHashedPassword(check.Parola,parola))
                {
                    if (check.IsActive)
                    {
                        Session["Kullanici"] = check;
                        
                        HttpCookie acct = new HttpCookie("acct","Username="+check.KullaniciAdi + "&Password="+check.Parola)
                        {
                            Expires = DateTime.Now.AddMonths(1)
                        };

                        HttpContext.Response.Cookies.Add(acct);

                        Session["_loginTryCount"] = 0;

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
        public JsonResult TakipciKaydet(string email)
        {
            string message = "Lütfen düzgün bir mail adresi girin";

            Session["_takipciDeneme"] = Convert.ToInt32(Session["_takipciDeneme"]) + 1;

            if (Convert.ToInt32(Session["_takipciDeneme"]) > 5)
            {
                message = "Cok fazla deneme yaptınız biraz dinlenin";
                return Json(message);
            }
            bool check = Portal.IsValidMail(email);

            if (check)
            {
                Takipciler isMailExists = _db.Subscribers.FirstOrDefault(x => x.Email == email);
                if (isMailExists != null)
                {
                    message = "zaten var";
                }
                else
                {
                    Takipciler yeniTakipci = new Takipciler()
                    {
                        Email = email,
                        KayitTarihi = DateTime.Now.ToString("dd-MM-yyyy"),
                        isActive = true,
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

        public ActionResult UnSubscribe(Guid? deActive) //TODO burayı test et
        {
            if (deActive == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var takipci = _db.Subscribers.FirstOrDefault(x => x.DelToken == deActive);

            if (takipci != null)
            {
                if (!takipci.isActive)
                {
                    ViewBag.ResultMessage = "Takipci listemizden zaten çıkarıldınız.";
                }
                else
                {
                    takipci.isActive = false;
                    ViewBag.ResultMessage = "Takipci listemizden başarıyla çıkarıldınız.";
                    _db.SaveChanges();
                    
                }
            }
            else
            {
                ViewBag.ResultMessage = "Takipci listemizde kisi bulunamadı.";
            }
            return View();
        }
        public ActionResult UserActivate(Guid? activateId)
        {
            if (activateId == null)
            {
                return RedirectToAction("Index", "Home");
            }

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
                }
            }
            else
            {
                ViewBag.ResultMessage = "Aktifleştirilecek kullanıcı bulunamadı !";
            }
            return View();
        }

        [AuthUser]
        public ActionResult LogOut()
        {
            Session.Clear();
            var cookie = HttpContext.Request.Cookies.Get("acct");
            cookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Response.Cookies.Add(cookie);
            return RedirectToAction("Index", "Home");
        }

        [NotAccessibleByLoggedInUser]
        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [NotAccessibleByLoggedInUser]
        [ValidateAntiForgeryToken]
        public ActionResult ForgetPassword(string kullaniciAdi)
        {
            Session["_forgetPasswordTryCount"] = Convert.ToInt32(Session["_forgetPasswordTryCount"])+1;
            ViewBag.UserTryCount = Session["_forgetPasswordTryCount"];

            if (Convert.ToInt32(Session["_forgetPasswordTryCount"]) > 5)
            {
                ModelState.AddModelError("", "Cok fazla deneme yaptin biraz dinlen");
                return View();
            }
            var user = _db.Kullanicilar.FirstOrDefault(x => x.KullaniciAdi == kullaniciAdi);
            if (user == null)
            {
                ModelState.AddModelError("","Boyle bir kullanici bulunadi");
                
            }
            else
            {
                ViewBag.Success = "basarili";
                Session["_forgetPasswordTryCount"] = 0;
                Portal.KullaniciSifreReset(user);

            }
            return View();
        }
        [HttpGet]
        public ActionResult PasswordReset(Guid? reset)
        {
            if (reset == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Kullanici active = _db.Kullanicilar.FirstOrDefault(x => x.PasswordReset == reset);

            if (active != null)
            {
                Session["reset"] = active;
                return View(new KullaniciGuncelleme());
            }
            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordReset(KullaniciGuncelleme updateInfo)
        {
            if (updateInfo.YeniParola.Equals(updateInfo.YeniParolaOnay) && Session["reset"] != null)
            {
                var changeUser = Session["reset"] as Kullanici;
                var updateUser = _db.Kullanicilar.FirstOrDefault(x => x.KullaniciAdi == changeUser.KullaniciAdi);
                updateUser.Parola = Crypto.HashPassword(updateInfo.YeniParola);
                updateUser.PasswordReset =  Guid.NewGuid();
                _db.SaveChanges();
                Session["reset"] = null;
                return RedirectToAction("UserPasswordResetSuccess", "Home");
            }
            else
            {
                ModelState.AddModelError("","Şifreler eşleşmiyor");
            }
           
            return View();
        }

        public ActionResult UserPasswordResetSuccess()
        {
            ViewBag.ResultMessage =
                "Şifreniz başarıyla değiştirildi.Artık yeni şifrenizle giriş yapabilirsiniz.";

            return View();
        }

        [ValidateInput(false)]
        [HttpGet]
        public ActionResult Arama(string query)
        {
            query = HttpUtility.HtmlEncode(query);
            ViewBag.AramaString = query;
            if (!String.IsNullOrWhiteSpace(query) && query.Length >= 3)
            {
                var aramaSonuclari = _db.Haberler.Where(x => x.Tags.ToLower().Contains(query)).OrderByDescending(x=>x.ID).ToList();
                if (aramaSonuclari.Count == 0)
                {
                    aramaSonuclari = _db.Haberler.Where(x => x.HaberIcerik.ToLower().Contains(query) ||  x.HaberBaslik.ToLower().Contains(query) ).OrderByDescending(x=>x.ID).ToList();
                    return View(aramaSonuclari);

                }

                return View(aramaSonuclari);
            }

            return RedirectToAction("Index","Home");
        }

        [ValidateInput(false)]
        [HttpGet]
        public ActionResult Kategori(string id) // id -> kategori ismi
        {
            id = HttpUtility.HtmlEncode(id);
            if (!String.IsNullOrWhiteSpace(id))
            {
                var checkIfExists = _db.Kategoriler.FirstOrDefault(x => x.KategoriIsim == id);
                if (checkIfExists != null)
                {
                    var sonuclar = _db.Haberler.Where(x => x.Kategorisi.KategoriIsim == id).OrderByDescending(x=> x.ID).ToList();
                    ViewBag.Kategori = checkIfExists;
                    return View(sonuclar);
                }
                
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
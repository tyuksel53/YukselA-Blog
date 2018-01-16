using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using yA_Blog.Areas.Blog.Filter;
using yA_Blog.Areas.Blog.Library;
using yA_Blog.Areas.Blog.Models;
using yA_Blog.Areas.Blog.Models.Managers;

namespace yA_Blog.Areas.Blog.Controllers
{
    [AuthAdmin]
    public class AdminController : Controller
    {
        readonly DatabaseContext _dB = new DatabaseContext();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult HaberEkle()
        {
            ViewBag.Kategoriler = Portal.Katagorileri_Getir(-1);
            return View(new Haber());
        }

        [HttpPost]
        public ActionResult HaberEkle(Haber model,int kategoriId)
        {
            System.Threading.Thread.Sleep(3000); // burayı değiş

            ViewBag.Kategoriler = Portal.Katagorileri_Getir(kategoriId);

            if (ModelState.IsValid)
            {
                model.Kategorisi = (from s in _dB.Kategoriler where s.ID == kategoriId select s).FirstOrDefault();

                if (model.Kategorisi == null)
                {
                    ModelState.AddModelError("", "Lütfen Kategori Seçiniz");
                    return PartialView("_HaberPartialView", model);
                }

                model.HaberYayinlamaTarih = DateTime.Now.ToString("dd-MM-yyyy");

                model.Yazar = Session["Kullanici"] as Kullanici;;

                ViewBag.SuccessAdd = true;

                _dB.Haberler.Add(model);
                _dB.SaveChanges();

                //todo bu kısımda takipcilere mesaj atılacak

                return PartialView("_HaberPartialView", new Haber() );
            }
            else
            {
                return PartialView("_HaberPartialView", model);
            }
        }
        public ActionResult Haberler(int? page)
        {
            Repository<Haber> repo = new Repository<Haber>();
            List<Haber> haberler = repo.SayfalariGetir(page,(x => x.ID) );

            if (haberler == null)
            {
                return RedirectToAction("Haberler","Admin", new {Area = "Blog", page = 1});
            }

            ViewBag.HaberCount = repo.IcerikSayisi(x => x.ID);

            return View(haberler);
        }

        [HttpPost]
        public JsonResult Haber_Sil(int silinecekId)
        {
            System.Threading.Thread.Sleep(3000);
            Haber silenecekHaber = _dB.Haberler.FirstOrDefault(s => s.ID == silinecekId);
            if(silenecekHaber == null)
            {
                return Json(false);
            }else
            {
                _dB.Haberler.Remove(silenecekHaber);
                _dB.SaveChanges();
                return Json(true);
            }

        }

        [HttpGet]
        public ActionResult HaberGuncelle(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Haberler","Admin");
            }
            Haber haberGuncelle = _dB.Haberler.FirstOrDefault(x => x.ID == id);
            if(haberGuncelle == null)
            {
                return HttpNotFound();
            }else
            {
                ViewBag.ID = id;
                ViewBag.Kategoriler = Portal.Katagorileri_Getir(haberGuncelle.Kategorisi.ID);
                haberGuncelle.HaberIcerik = HttpUtility.HtmlDecode(haberGuncelle.HaberIcerik);

                return View(haberGuncelle);
            }
            
        }

        [HttpPost]
        public ActionResult HaberGuncelle(Haber model, int kategoriId)
        {
            System.Threading.Thread.Sleep(3000);

            ViewBag.Kategoriler = Portal.Katagorileri_Getir(kategoriId);

            if (ModelState.IsValid)
            {
                Haber update = _dB.Haberler.FirstOrDefault(x => x.ID == model.ID);

                if (update == null)
                {
                    ModelState.AddModelError("", "bir şeyler ters gitti :(");
                    return PartialView("_HaberPartialView", model);
                }
                
                Kategori updateKategori = (from s in _dB.Kategoriler where s.ID == kategoriId select s).FirstOrDefault();

                if (updateKategori == null)
                {
                    ModelState.AddModelError("", "Lütfen Kategori Seçiniz");
                    return PartialView("_HaberPartialView", model);
                }

                update.HaberBaslik = model.HaberBaslik;
                update.HaberOzet = model.HaberOzet;
                update.HaberIcerik = HttpUtility.HtmlDecode(model.HaberIcerik);
                update.HaberResimUrl = model.HaberResimUrl;
                update.Tags = model.Tags;
                update.Kategorisi = updateKategori;
                update.Taslak = model.Taslak;

                ViewBag.SuccessUpdate = true;

                _dB.SaveChanges();

                return PartialView("_HaberPartialView",update);
            }
            else
            {
                return PartialView("_HaberPartialView", model);
            }
        }
        [HttpGet]
        public ActionResult KategoriEkle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KategoriEkle(Kategori model)
        {
            if(ModelState.IsValid)
            {

                Kategori yeniKategori = _dB.Kategoriler.FirstOrDefault(x => x.KategoriIsim == model.KategoriIsim);

                if (yeniKategori != null)
                {
                    ModelState.AddModelError("", "Bu isimde kategori zaten mevcut. Farklı bir isimle tekrer deneyin");
                    return View(model);
                }

                yeniKategori = model;

                _dB.Kategoriler.Add(yeniKategori);

                _dB.SaveChanges();
                CacheHelper.Remove("kategori-cache");
                ViewBag.Success = true;

                return View(model);

            }else
            {
                return View(model);
            }
        }
        [HttpGet]
        public ActionResult Kategoriler(int? page)
        {
            Repository<Kategori> repo = new Repository<Kategori>();
            List<Kategori> kategoriler = repo.SayfalariGetir(page, (x => x.ID));

            if (kategoriler == null)
            {
                return RedirectToAction("Kategoriler", "Admin", new { Area = "Blog", page = 1 });
            }

            ViewBag.KategoriCount = repo.IcerikSayisi(x => x.ID);

            return View(kategoriler);
        }
        public JsonResult KategoriSil(int silinecekId)
        {
            Kategori silKategori = _dB.Kategoriler.FirstOrDefault(x => x.ID == silinecekId);
            if(silKategori == null)
            {
                return Json(false);
            }

            _dB.Kategoriler.Remove(silKategori);
            _dB.SaveChanges();
            CacheHelper.Remove("kategori-cache");
            return Json(true);
        }
        [HttpGet]
        public ActionResult KategoriGuncelle(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Kategoriler","Admin", new { Area = "blog" });
            }
            Kategori guncelle = _dB.Kategoriler.FirstOrDefault(x => x.ID == id);

            if(guncelle == null)
            {
                return HttpNotFound();
            }else
            {
                ViewBag.ID = id;
                return View(guncelle);
            }
        }
        [HttpPost]
        public ActionResult KategoriGuncelle(Kategori model)
        {
            if(ModelState.IsValid)
            {
                Kategori guncelle = _dB.Kategoriler.FirstOrDefault(x => x.ID == model.ID);
                if(guncelle == null)
                {
                    ModelState.AddModelError("", "Bir seyler ters gitti");
                    return View(model);
                }

                Kategori check = _dB.Kategoriler.FirstOrDefault(x => x.KategoriIsim == model.KategoriIsim);

                if(check == null)
                {
                    guncelle.KategoriIsim = model.KategoriIsim;
                    guncelle.KategoriResim = model.KategoriResim;
                    _dB.SaveChanges();
                    ViewBag.Success = true;
                    ViewBag.ID = guncelle.ID;
                    CacheHelper.Remove("kategori-cache");
                    return View(model);
                }else
                {
                    if(check.ID == guncelle.ID)
                    {
                        guncelle.KategoriIsim = model.KategoriIsim;
                        guncelle.KategoriResim = model.KategoriResim;
                        _dB.SaveChanges();
                        ViewBag.Success = true;
                        ViewBag.ID = guncelle.ID;
                        CacheHelper.Remove("kategori-cache");
                        return View(model);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Bu isimde zaten kategori mevcut. Farklı bir isimle tekrar deneyin");
                        return View(model);
                    }
                    
                }

            }else
            {
                return View(model);
            }
        }
        [HttpGet]
        [ExcFilter]
        public ActionResult Uploads(int? page)
        {
            Repository<Uploads> repo = new Repository<Uploads>();
            List<Uploads> uploads = repo.SayfalariGetir(page, (x => x.ID));

            if (uploads == null)
            {
                return RedirectToAction("Uploads", "Admin", new { Area = "Blog", page = 1 });
            }

            ViewBag.UploadsCount = repo.IcerikSayisi(x => x.ID);
            ViewBag.Uploads = uploads;

            return View();
        }

        [HttpPost]
        public JsonResult Uploads(HttpPostedFileBase[] files)
        {
            List<Uploads> dosyalar = new List<Uploads>();
            string uploadStatus = "";
            if (ModelState.IsValid)
            {
                foreach (HttpPostedFileBase file in files)
                {
                    
                    if (file != null)
                    {
                        Uploads upload = new Uploads
                        {
                            DosyaAdı = Path.GetFileName(file.FileName)
                        };
                        for (int i = 0; i < 1000; i++)
                        {
                            var check = _dB.Uploads.FirstOrDefault(x => x.DosyaAdı == upload.DosyaAdı);
                            if (check == null)
                            {
                                upload.DosyaUzantisi = Path.GetExtension(file.FileName);
                                upload.YuklenmeTarihi = DateTime.Now.ToString("dd-MM-yyyy");

                                upload.DosyaYolu = Path.Combine(Server.MapPath("~/Areas/Blog/Uploads"),upload.DosyaAdı);
                                file.SaveAs(upload.DosyaYolu);

                                _dB.Uploads.Add(upload);
                                _dB.SaveChanges();
                                dosyalar.Add(upload);
                                uploadStatus = files.Count().ToString() + " tane dosya başarıyla yüklendi";
                                break;
                            }
                            else
                            {
                                upload.DosyaAdı = upload.DosyaAdı.Replace("(" + (i - 1) + ")", "");
                                upload.DosyaAdı = "(" + i + ")" + upload.DosyaAdı;
                            }
                        }
                        
                    }
                }
            }
            Tuple<string,List<Uploads>> message = new Tuple<string, List<Uploads>>(uploadStatus,dosyalar);
            return  Json(message);
        }

        [HttpPost]
        public JsonResult DosyaSil(int silinecekId)
        {
            System.Threading.Thread.Sleep(3000);
            Uploads dosya = _dB.Uploads.FirstOrDefault(x => x.ID == silinecekId);
            if (dosya != null)
            {
                try
                {
                    _dB.Uploads.Remove(dosya);
                    _dB.SaveChanges();

                    string strFileFullPath = dosya.DosyaYolu;

                    if (System.IO.File.Exists(strFileFullPath))
                    {
                        System.IO.File.Delete(strFileFullPath);
                    }
                    return Json(true);
                }
                catch (Exception)
                {
                    return Json(false);
                }
                
            }
            else
            {
                return Json(false);
            }
        }


        public ActionResult Takipciler(int? page)
        {
            Repository<Takipciler> repo = new Repository<Takipciler>();
            List<Takipciler> takipciler = repo.SayfalariGetir(page, (x => x.ID));

            if (takipciler == null)
            {
                return RedirectToAction("Takipciler", "Admin", new { Area = "Blog", page = 1 });
            }

            ViewBag.SubscribersCount = repo.IcerikSayisi(x => x.ID);

            return View(takipciler);
        }

        [HttpPost]
        public JsonResult TakipciSil(int silinecekId)
        {
            System.Threading.Thread.Sleep(3000);
            var silinecekTakipci = _dB.Subscribers.FirstOrDefault(x => x.ID == silinecekId);

            if (silinecekTakipci == null)
            {
                return Json(false);
            }
            else
            {
                _dB.Subscribers.Remove(silinecekTakipci);
                _dB.SaveChanges();
                return Json(true);
            }
        }

        [HttpGet]
        public ActionResult Kullanicilar(int? page)
        {
            Repository<Kullanici> repo = new Repository<Kullanici>();
            List<Kullanici> kullanicilar = repo.SayfalariGetir(page, (x => x.ID));

            if (kullanicilar == null)
            {
                return RedirectToAction("Kullanicilar", "Admin", new { Area = "Blog", page = 1 });
            }

            ViewBag.KullaniciCount = repo.IcerikSayisi(x => x.ID);

            return View(kullanicilar);
        }

        [HttpPost]
        public JsonResult KullaniciSil(int silinecekId)
        {
            Kullanici silinecekKullanici = _dB.Kullanicilar.FirstOrDefault(x => x.ID == silinecekId);
            if (silinecekKullanici != null)
            {
                _dB.Kullanicilar.Remove(silinecekKullanici);
                _dB.SaveChanges();
                return Json(true);
            }

            return Json(false);
        }

        [HttpGet]
        public ActionResult KullaniciEkle()
        {
            ViewBag.Roller = Portal.Roller();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KullaniciEkle(Kullanici model, int role)
        {
            ViewBag.Roller = Portal.Roller();

            if (ModelState.IsValid)
            {
                if (role < 0 || role > 2)
                {
                    ModelState.AddModelError("","Lütfen düzgün bir rol seçin");
                    return View();
                }

                Kullanici checkExists = _dB.Kullanicilar.FirstOrDefault(x => x.KullaniciAdi == model.KullaniciAdi);
                if (checkExists != null)
                {
                    ModelState.AddModelError("","Bu kullanici ismi zaten mevcut");
                    return View();
                }

                model.Parola = Crypto.HashPassword(model.Parola);
                model.ActivateGuid = Guid.NewGuid();
                model.PasswordReset = Guid.NewGuid();
                model.ImgUrl = "";
                model.Role = role == 1 ? "user" : "admin";
                model.IsActive = model.Role.Equals("admin");
                ViewBag.Message = "Kullanici Eklendi";

                _dB.Kullanicilar.Add(model);
                _dB.SaveChanges();

                return View();
            }

            return View();
        }
    }
}
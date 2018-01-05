using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using yA_Blog.Areas.Blog.Filter;
using yA_Blog.Areas.Blog.Models;
using yA_Blog.Areas.Blog.Models.Managers;

namespace yA_Blog.Areas.Blog.Controllers
{
    public class AdminController : Controller
    {
        DatabaseContext dB = new DatabaseContext();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult HaberEkle()
        {
            ViewBag.Kategoriler = Katagorileri_Getir(-1);
            return View(new Haber());
        }

        [HttpPost]
        public ActionResult HaberEkle(Haber model,int kategoriId)
        {
            System.Threading.Thread.Sleep(3000); // burayı değiş

            ViewBag.Kategoriler = Katagorileri_Getir(kategoriId);

            if (ModelState.IsValid)
            {
                model.Kategorisi = (from s in dB.Kategoriler where s.ID == kategoriId select s).FirstOrDefault();

                if (model.Kategorisi == null)
                {
                    ModelState.AddModelError("", "Lütfen Kategori Seçiniz");
                    return PartialView("_HaberPartialView", model);
                }

                model.HaberYayinlamaTarih = DateTime.Now.ToString("dd-MM-yyyy");

                model.Yazar = (from s in dB.Kullanicilar select s).FirstOrDefault();

                ViewBag.SuccessAdd = true;

                dB.Haberler.Add(model);
                dB.SaveChanges();

                return PartialView("_HaberPartialView", new Haber() );
            }
            else
            {
                return PartialView("_HaberPartialView", model);
            }
        }
        public ActionResult HaberleriListele(int? page)
        {
            if (page == null)
            {
                return RedirectToAction("HaberleriListele", "Admin", new { Area = "blog", page = 1 });
            }

            if (page >= 1)
            {
                var total = dB.Haberler.Select(p => p.ID).Count();
                ViewBag.HaberCount = total;
                int sayfaSayisi = (total / 10) + 1;

                if (sayfaSayisi < page)
                {
                    return RedirectToAction("HaberleriListele", "Admin", new { Area = "blog", page = 1 });
                }
                else
                {
                    int skip = (int)(page - 1) * 10;

                    var result = dB.Haberler.OrderBy(x => x.ID).
                        Skip(skip).
                        Take(10).
                        ToList();
                    return View(result);

                }
            }
            else
            {
                return RedirectToAction("HaberleriListele", "Admin", new { Area = "blog", page = 1 });
            }
        }

        [HttpPost]
        public JsonResult Haber_Sil(int silinecekId)
        {
            System.Threading.Thread.Sleep(3000);
            Haber silenecekHaber = dB.Haberler.Where(s => s.ID == silinecekId).FirstOrDefault();
            if(silenecekHaber == null)
            {
                return Json(false);
            }else
            {
                dB.Haberler.Remove(silenecekHaber);
                dB.SaveChanges();
                return Json(true);
            }

        }

        [HttpGet]
        public ActionResult HaberGuncelle(int? ID)
        {
            if(ID == null)
            {
                ID = 1;
            }
            Haber haberGuncelle = dB.Haberler.Where(x => x.ID == ID).FirstOrDefault();
            if(haberGuncelle == null)
            {
                return HttpNotFound();
            }else
            {
                ViewBag.ID = ID;
                ViewBag.Kategoriler = Katagorileri_Getir(haberGuncelle.Kategorisi.ID);
                haberGuncelle.HaberIcerik = HttpUtility.HtmlDecode(haberGuncelle.HaberIcerik);

                return View(haberGuncelle);
            }
            
        }
        [HttpPost]
        public ActionResult HaberGuncelle(Haber model, int kategoriId)
        {
            System.Threading.Thread.Sleep(3000);

            ViewBag.Kategoriler = Katagorileri_Getir(kategoriId);

            if (ModelState.IsValid)
            {
                Haber update = dB.Haberler.Where(x => x.ID == model.ID).FirstOrDefault();

                if (update == null)
                {
                    ModelState.AddModelError("", "bir şeyler ters gitti :(");
                    return PartialView("_HaberPartialView", model);
                }
                
                Kategori updateKategori = (from s in dB.Kategoriler where s.ID == kategoriId select s).FirstOrDefault();

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

                ViewBag.SuccessUpdate = true;

                dB.SaveChanges();

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

                Kategori yeniKategori = dB.Kategoriler.Where(x => x.KategoriIsım == model.KategoriIsım).FirstOrDefault();

                if (yeniKategori != null)
                {
                    ModelState.AddModelError("", "Bu isimde kategori zaten mevcut. Farklı bir isimle tekrer deneyin");
                    return View(model);
                }

                yeniKategori = model;

                dB.Kategoriler.Add(yeniKategori);

                dB.SaveChanges();
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
            if (page == null)
            {
                return RedirectToAction("Kategoriler", "Admin", new { Area = "blog", page = 1 });
            }

            if (page >= 1)
            {
                var total = dB.Kategoriler.Select(p => p.ID).Count();
                ViewBag.KategoriCount = total;
                int sayfaSayisi = (total / 10) + 1;

                if (sayfaSayisi < page)
                {
                    return RedirectToAction("Kategoriler", "Admin", new { Area = "blog", page = 1 });
                }
                else
                {
                    int skip = (int)(page - 1) * 10;

                    var result = dB.Kategoriler.OrderBy(x => x.ID).
                        Skip(skip).
                        Take(10).
                        ToList();
                    return View(result);

                }
            }
            else
            {
                return RedirectToAction("Kategoriler", "Admin", new { Area = "blog", page = 1 });
            }
        }
        public JsonResult KategoriSil(int silenecekId)
        {
            Kategori silKategori = dB.Kategoriler.Where(x => x.ID == silenecekId).FirstOrDefault();
            if(silKategori == null)
            {
                return Json(false);
            }

            dB.Kategoriler.Remove(silKategori);
            dB.SaveChanges();
            return Json(true);
        }
        [HttpGet]
        public ActionResult KategoriGuncelle(int? ID)
        {
            if(ID == null)
            {
                return RedirectToAction("Kategoriler","Admin", new { Area = "blog" });
            }
            Kategori guncelle = dB.Kategoriler.Where(x => x.ID == ID).FirstOrDefault();

            if(guncelle == null)
            {
                return HttpNotFound();
            }else
            {
                ViewBag.ID = ID;
                return View(guncelle);
            }
        }
        [HttpPost]
        public ActionResult KategoriGuncelle(Kategori model)
        {
            if(ModelState.IsValid)
            {
                Kategori guncelle = dB.Kategoriler.Where(x => x.ID == model.ID).FirstOrDefault();
                if(guncelle == null)
                {
                    ModelState.AddModelError("", "Bir seyler ters gitti");
                    return View(model);
                }

                Kategori check = dB.Kategoriler.Where(x => x.KategoriIsım == model.KategoriIsım).FirstOrDefault();

                if(check == null)
                {
                    guncelle.KategoriIsım = model.KategoriIsım;
                    guncelle.KategoriResim = model.KategoriResim;
                    dB.SaveChanges();
                    ViewBag.Success = true;
                    ViewBag.ID = guncelle.ID;
                    return View(model);
                }else
                {
                    if(check.ID == guncelle.ID)
                    {
                        guncelle.KategoriIsım = model.KategoriIsım;
                        guncelle.KategoriResim = model.KategoriResim;
                        dB.SaveChanges();
                        ViewBag.Success = true;
                        ViewBag.ID = guncelle.ID;
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
            if (page == null)
            {
                return RedirectToAction("Uploads", "Admin", new { Area = "blog", page = 1 });
            }

            if (page >= 1)
            {
                var total = dB.Kategoriler.Select(p => p.ID).Count();

                ViewBag.UploadsCount = total;

                int sayfaSayisi = (total / 10) + 1;

                if (sayfaSayisi < page)
                {
                    return RedirectToAction("Uploads", "Admin", new { Area = "blog", page = 1 });
                }
                else
                {
                    int skip = (int)(page - 1) * 10;

                    ViewBag.Uploads = dB.Uploads.OrderBy(x => x.ID).
                        Skip(skip).
                        Take(10).
                        ToList();
                    return View();

                }
            }
            else
            {
                return RedirectToAction("Kategoriler", "Admin", new { Area = "blog", page = 1 });
            }
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
                            var check = dB.Uploads.FirstOrDefault(x => x.DosyaAdı == upload.DosyaAdı);
                            if (check == null)
                            {
                                upload.DosyaUzantisi = file.ContentType;
                                upload.YuklenmeTarihi = DateTime.Now;

                                upload.DosyaYolu = Path.Combine(Server.MapPath("~/Areas/Blog/Uploads"),upload.DosyaAdı);
                                file.SaveAs(upload.DosyaYolu);

                                dB.Uploads.Add(upload);
                                dB.SaveChanges();
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
        public List<SelectListItem> Katagorileri_Getir(int ID)
        {
            List<SelectListItem> kisilerListe =
            (from s in CacheHelper.KategoriGet()
             select new SelectListItem()
             {
                 Text = s.KategoriIsım,
                 Value = s.ID.ToString(),
                 Selected = s.ID == ID ? true : false
             }
            ).ToList();

            return kisilerListe;
        }
    }
}
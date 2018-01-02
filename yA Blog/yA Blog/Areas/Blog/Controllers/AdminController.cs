using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using yA_Blog.Areas.Blog.Models;
using yA_Blog.Areas.Blog.Models.Managers;

namespace yA_Blog.Areas.Blog.Controllers
{
    public class AdminController : Controller
    {
        DatabaseContext db = new DatabaseContext();

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
        public ActionResult HaberEkle(Haber Model,int KategoriID)
        {
            System.Threading.Thread.Sleep(3000);

            ViewBag.Kategoriler = Katagorileri_Getir(KategoriID);

            if (ModelState.IsValid)
            {
                Model.Kategorisi = (from s in db.Kategoriler where s.ID == KategoriID select s).FirstOrDefault();

                if (Model.Kategorisi == null)
                {
                    ModelState.AddModelError("", "Lütfen Kategori Seçiniz");
                    return PartialView("_HaberPartialView", Model);
                }

                Model.HaberYayinlamaTarih = DateTime.Now.ToString("dd-MM-yyyy");

                Model.Yazar = (Kullanici)(from s in db.Kullanicilar select s).FirstOrDefault();

                ViewBag.SuccessAdd = true;

                db.Haberler.Add(Model);
                db.SaveChanges();

                return PartialView("_HaberPartialView", new Haber() );
            }
            else
            {
                return PartialView("_HaberPartialView", Model);
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
                var total = db.Haberler.Select(p => p.ID).Count();
                ViewBag.HaberCount = total;
                int sayfa_sayisi = (total / 10) + 1;

                if (sayfa_sayisi < page)
                {
                    return RedirectToAction("HaberleriListele", "Admin", new { Area = "blog", page = 1 });
                }
                else
                {
                    int skip = (int)(page - 1) * 10;

                    var result = db.Haberler.OrderBy(x => x.ID).
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
        public JsonResult Haber_Sil(int silinecek_ID)
        {
            System.Threading.Thread.Sleep(3000);
            Haber silenecekHaber = db.Haberler.Where(s => s.ID == silinecek_ID).FirstOrDefault();
            if(silenecekHaber == null)
            {
                return Json(false);
            }else
            {
                db.Haberler.Remove(silenecekHaber);
                db.SaveChanges();
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
            Haber haber_guncelle = db.Haberler.Where(x => x.ID == ID).FirstOrDefault();
            if(haber_guncelle == null)
            {
                return HttpNotFound();
            }else
            {
                ViewBag.ID = ID;
                ViewBag.Kategoriler = Katagorileri_Getir(haber_guncelle.Kategorisi.ID);
                haber_guncelle.HaberIcerik = HttpUtility.HtmlDecode(haber_guncelle.HaberIcerik);
                return View(haber_guncelle);
            }
            
        }
        [HttpPost]
        public ActionResult HaberGuncelle(Haber Model, int KategoriID)
        {
            System.Threading.Thread.Sleep(3000);

            ViewBag.Kategoriler = Katagorileri_Getir(KategoriID);

            if (ModelState.IsValid)
            {
                Haber update = db.Haberler.Where(x => x.ID == Model.ID).FirstOrDefault();

                if (update == null)
                {
                    ModelState.AddModelError("", "bir şeyler ters gitti :(");
                    return PartialView("_HaberPartialView", Model);
                }
                
                Kategori updateKategori = (from s in db.Kategoriler where s.ID == KategoriID select s).FirstOrDefault();

                if (updateKategori == null)
                {
                    ModelState.AddModelError("", "Lütfen Kategori Seçiniz");
                    return PartialView("_HaberPartialView", Model);
                }

                update.HaberBaslik = Model.HaberBaslik;
                update.HaberOzet = Model.HaberOzet;
                update.HaberIcerik = HttpUtility.HtmlDecode(Model.HaberIcerik);
                update.HaberResimUrl = Model.HaberResimUrl;
                update.Tags = Model.Tags;
                update.Kategorisi = updateKategori;

                ViewBag.SuccessUpdate = true;

                db.SaveChanges();

                return PartialView("_HaberPartialView",update);
            }
            else
            {
                return PartialView("_HaberPartialView", Model);
            }
        }
        [HttpGet]
        public ActionResult KategoriEkle()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KategoriEkle(Kategori Model)
        {
            if(ModelState.IsValid)
            {

                Kategori yeniKategori = db.Kategoriler.Where(x => x.KategoriIsım == Model.KategoriIsım).FirstOrDefault();

                if (yeniKategori != null)
                {
                    ModelState.AddModelError("", "Bu isimde kategori zaten mevcut. Farklı bir isimle tekrer deneyin");
                    return View(Model);
                }

                yeniKategori = Model;

                db.Kategoriler.Add(yeniKategori);

                db.SaveChanges();
                ViewBag.Success = true;

                return View(Model);

            }else
            {
                return View(Model);
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
                var total = db.Kategoriler.Select(p => p.ID).Count();
                ViewBag.KategoriCount = total;
                int sayfa_sayisi = (total / 10) + 1;

                if (sayfa_sayisi < page)
                {
                    return RedirectToAction("HaberleriListele", "Admin", new { Area = "blog", page = 1 });
                }
                else
                {
                    int skip = (int)(page - 1) * 10;

                    var result = db.Kategoriler.OrderBy(x => x.ID).
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
        public JsonResult KategoriSil(int silenecek_Id)
        {
            Kategori sil_kategori = db.Kategoriler.Where(x => x.ID == silenecek_Id).FirstOrDefault();
            if(sil_kategori == null)
            {
                return Json(false);
            }

            db.Kategoriler.Remove(sil_kategori);
            db.SaveChanges();
            return Json(true);
        }
        [HttpGet]
        public ActionResult KategoriGuncelle(int? ID)
        {
            if(ID == null)
            {
                return RedirectToAction("Kategoriler","Admin", new { Area = "blog" });
            }
            Kategori guncelle = db.Kategoriler.Where(x => x.ID == ID).FirstOrDefault();

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
        public ActionResult KategoriGuncelle(Kategori Model)
        {
            if(ModelState.IsValid)
            {
                Kategori guncelle = db.Kategoriler.Where(x => x.ID == Model.ID).FirstOrDefault();
                if(guncelle == null)
                {
                    ModelState.AddModelError("", "Bir seyler ters gitti");
                    return View(Model);
                }

                Kategori check = db.Kategoriler.Where(x => x.KategoriIsım == Model.KategoriIsım).FirstOrDefault();

                if(check == null)
                {
                    guncelle.KategoriIsım = Model.KategoriIsım;
                    guncelle.KategoriResim = Model.KategoriResim;
                    db.SaveChanges();
                    ViewBag.Success = true;
                    ViewBag.ID = guncelle.ID;
                    return View(Model);
                }else
                {
                    if(check.ID == guncelle.ID)
                    {
                        guncelle.KategoriIsım = Model.KategoriIsım;
                        guncelle.KategoriResim = Model.KategoriResim;
                        db.SaveChanges();
                        ViewBag.Success = true;
                        ViewBag.ID = guncelle.ID;
                        return View(Model);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Bu isimde zaten kategori mevcut. Farklı bir isimle tekrar deneyin");
                        return View(Model);
                    }
                    
                }

            }else
            {
                return View(Model);
            }
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
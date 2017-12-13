using System;
using System.Collections.Generic;
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
            List<SelectListItem> kisilerListe =
            (from s in CacheHelper.KategoriGet() select new SelectListItem()
            {
                Text = s.KategoriIsım,
                Value = s.ID.ToString()
            }
            ).ToList();

            ViewBag.Kategoriler = kisilerListe;
            return View(new Haber());
        }

        [HttpPost]
        public ActionResult HaberEkle(Haber Model,int KategoriID)
        {
            var mundi = db.Haberler.ToList();
            System.Threading.Thread.Sleep(3000);

            List<SelectListItem> kisilerListe =
            (from s in CacheHelper.KategoriGet() select new SelectListItem()
             {
                 Text = s.KategoriIsım,
                 Value = s.ID.ToString()
             }
            ).ToList();

            ViewBag.Kategoriler = kisilerListe;

            if (ModelState.IsValid)
            {
                Model.Kategorisi = (from s in db.Kategoriler where s.ID == KategoriID select s).FirstOrDefault();

                if (Model.Kategorisi == null)
                {
                    ModelState.AddModelError("", "Lütfen Kategori Seçiniz");
                    return PartialView("_HaberCreatePartialView", Model);
                }

                Model.HaberYayinlamaTarih = DateTime.Now.ToString("dd-MM-yyyy");

                Model.Yazar = (Kullanici)(from s in db.Kullanicilar select s).FirstOrDefault();

                ViewBag.Success = true;

                db.Haberler.Add(Model);
                db.SaveChanges();

                return PartialView("_HaberCreatePartialView", new Haber() );
            }
            else
            {
                return PartialView("_HaberCreatePartialView", Model);
            }
        }
        public ActionResult HaberleriListele(int? rowNum)
        {
            if (rowNum == null)
            {
                return RedirectToAction("HaberleriListele", "Admin", new { Area = "blog", rowNum = 1 });
            }

            if (rowNum >= 1)
            {
                var total = db.Haberler.Select(p => p.ID).Count();
                ViewBag.HaberCount = total;
                int sayfa_sayisi = (total / 10) + 1;

                if (sayfa_sayisi < rowNum)
                {
                    return RedirectToAction("HaberleriListele", "Admin", new { Area = "blog", rowNum = 1 });
                }
                else
                {
                    int skip = (int)(rowNum - 1) * 10;

                    var result = db.Haberler.OrderBy(x => x.ID).
                        Skip(skip).
                        Take(10).
                        ToList();
                    return View(result);

                }
            }
            else
            {
                return RedirectToAction("HaberleriListele", "Admin", new { Area = "blog", rowNum = 1 });
            }
        }
    }
}
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
    }
}
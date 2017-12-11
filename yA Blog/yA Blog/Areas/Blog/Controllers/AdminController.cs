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
            (from s in db.Katagoriler.ToList() select new SelectListItem()
            {
                Text = s.KatagoriIsım,
                Value = s.ID.ToString()
                }
            ).ToList();
            ViewBag.Katagoriler = kisilerListe;
            return View(new Haber());
        }

        [HttpPost]
        public ActionResult HaberEkle(Haber Model,int KatagoriID)
        {
            if(String.IsNullOrWhiteSpace(Model.Tags[0]))
            {
                string[] tagler = Model.Tags[0].Split(',');
                Model.Tags = tagler;
            }
            Model.Katagorisi = (from s in db.Katagoriler where s.ID == KatagoriID select s).FirstOrDefault();

            if(ModelState.IsValid)
            {
                var zundi = 5;
            }
            
            return Json("mundi", JsonRequestBehavior.AllowGet);
        }
    }
}
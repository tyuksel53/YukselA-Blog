using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace yA_Blog.Areas.Blog.Controllers
{
    public class PostController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}
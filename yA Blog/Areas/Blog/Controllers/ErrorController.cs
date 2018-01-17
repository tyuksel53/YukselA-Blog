using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace yA_Blog.Areas.Blog.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Page404()
        {
            Response.StatusCode = 404;
            return View();
        }
    }
}
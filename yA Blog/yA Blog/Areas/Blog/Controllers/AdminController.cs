﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace yA_Blog.Areas.Blog.Controllers
{
    public class AdminController : Controller
    {
        // GET: Blog/Admin
        public ActionResult Index()
        {
            return View();
        }
    }
}
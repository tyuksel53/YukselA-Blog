using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using yA_Blog.Areas.Blog.Models.Managers;
using yA_Blog.Areas.Blog.PageModels;

namespace yA_Blog.Areas.Blog.Controllers
{
    public class PostController : Controller
    {
        private readonly DatabaseContext _dB = new DatabaseContext();

        [HttpGet]
        public ActionResult Haber(string id)
        {
            
            try
            {
                string[] parse = id.Split('-');
                int postId = Convert.ToInt32(parse[parse.Length - 1]);

                var post = _dB.Haberler.FirstOrDefault(x => x.ID == postId);
                if (post == null)
                {
                    return HttpNotFound();
                }
                PostPage requestedPage = new PostPage
                {
                    Post = post,
                    Yorumlar = _dB.Yorumlar.Where(x => x.PostId == postId).ToList()
                };
                return View(requestedPage);
            }
            catch (Exception ex)
            {
                return HttpNotFound();
            }
        }
    }
}
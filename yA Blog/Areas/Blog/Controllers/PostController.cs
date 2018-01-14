using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using yA_Blog.Areas.Blog.Filter;
using yA_Blog.Areas.Blog.Models;
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
                TempData["currentPostId"] = postId;
                return View(requestedPage);
            }
            catch (Exception ex)
            {
                return HttpNotFound();
            }
        }

        [AuthUser]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public MvcHtmlString Yorum(string Description)
        {
            Description = HttpUtility.HtmlEncode(Description);
            string html = "";
            Kullanici commentOwner = Session["Kullanici"] as Kullanici;
            if (Description != null && Description.Length < 280 && Description.Length > 10 && TempData["currentPostId"] != null)
            {
                var yorum = new Yorum
                {
                    CommentTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
                    PostId = Convert.ToInt32(TempData["currentPostId"]),
                    UserName = commentOwner.KullaniciAdi,
                    Description = Description
                };
                
                _dB.Yorumlar.Add(yorum);
                _dB.SaveChanges();

                html = "<div class='media mb-4'>" +
                       "<img class='d-flex mr-3 rounded-circle' src='http://placehold.it/50x50' alt=''>" +
                       "<div class='media-body'>" +
                       $"<h5 class='mt-0' style='font-size: 21px'>{yorum.UserName}</h5>" +
                       $"<p class='meta'><i class='link-spacer'></i>{yorum.CommentTime}<i class='link-spacer'></i> Tarihinde yazdı.</p>" +
                       $"<p style = 'font-size:18px'>{yorum.Description}</p>" +
                       "</div>" +
                       "</div>";

                TempData["currentPostId"] = yorum.PostId;
            }
            else
            {
                html = "yorum uygun formatta degil";
            }
            
            return MvcHtmlString.Create(html);
        }
    }
}
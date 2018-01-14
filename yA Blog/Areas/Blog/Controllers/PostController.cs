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
                    Yorumlar = _dB.Yorumlar.Where(x => x.PostId == postId).ToList(),
                    AltYorumlar = _dB.AltYorumlar.Where(x=> x.PostId == postId).ToList()
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
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult YorumSil(int? silinecekId)
        {
            string result = "basarisiz";
            if (silinecekId == null)
            {
                return Json(result);
            }

            var silinecekYorum = _dB.Yorumlar.FirstOrDefault(x => x.ID == silinecekId);
            if (silinecekYorum == null)
            {
                return Json(result);
            }

            _dB.Yorumlar.Remove(silinecekYorum);
            _dB.SaveChanges();
            result = $"yorum_{silinecekId}";
            return Json(result);
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
            if (Description != null && Description.Length <= 280 && Description.Length >= 10 && TempData["currentPostId"] != null)
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
                string yorumSilButtonAdd = commentOwner.Role.Equals("admin")
                    ? "<a href = '#' style = 'font-size: 14px; margin-bottom: 10px;margin-top: 10px' class='btn btn-danger' onclick='yorumSilBaslat(" +
                      yorum.ID + ")'>Sil</a>"
                    : "";
                       html = $"<div id='yorum_{yorum.ID}' class='media mb-4'>" +
                       "<i class='fa fa-user fa-5x' style='margin-right:26px; '></i>" +
                       "<div class='media-body'>" +
                       $"<h5 class='mt-0' style='font-size: 21px'>{yorum.UserName}</h5>" +
                       $"<p class='meta'><i class='link-spacer'></i>{yorum.CommentTime}<i class='link-spacer'></i> Tarihinde yazdı.</p>" +
                       $"<p style = 'font-size:18px;margin-bottom:10px'>{yorum.Description}</p>" +
                       $"<p class='meta text-danger'><a href='#' onclick='yanitla({yorum.ID},{yorum.ID})' class='text-danger'><i class='fa fa-paper-plane-o'></i> Yanıtla</a></p>"+
                       yorumSilButtonAdd+
                       "</div>" +
                       "</div>";

                TempData["currentPostId"] = yorum.PostId;
            }
            
            return MvcHtmlString.Create(html);
        }
    }
}
using System;
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
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        [AuthUser]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AltYorumSil(int? silinecekId)
        {
            string result = "basarisiz";
            if (silinecekId == null)
            {
                return Json(result);
            }

            var silinecekAltYorum = _dB.AltYorumlar.FirstOrDefault(x => x.ID == silinecekId);
            if (silinecekAltYorum == null)
            {
                return Json(result);
            }
            _dB.AltYorumlar.Remove(silinecekAltYorum);
            _dB.SaveChanges();
            result = "altYorum_" + silinecekId;
            return Json(result);
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

            var altYorumlar = _dB.AltYorumlar.Where(x => x.RootCommentId == silinecekYorum.ID).ToList();

            if (altYorumlar.Count > 0)
            {
                foreach (var altYorum in altYorumlar)
                {
                    _dB.AltYorumlar.Remove(altYorum);
                }
            }

            _dB.Yorumlar.Remove(silinecekYorum);
            _dB.SaveChanges();
            result = $"yorum_{silinecekId}";
            return Json(result);
        }

        [AuthUser]
        [HttpPost]
        [ValidateInput(false)]
        public MvcHtmlString YorumCevap(string SubComment,int parentContainer,int replyContainer)
        {                                      
            /* TODO: Bildirim
             * replyContainer kullanılarak kullanıcılara bildirim gösterilecek.
             * sonraki versiyonlarda
             */
            System.Threading.Thread.Sleep(2000);
            string html = "";
            SubComment = HttpUtility.HtmlEncode(SubComment);
            Kullanici commnetOwner = Session["Kullanici"] as Kullanici;
            if (!String.IsNullOrWhiteSpace(SubComment) && SubComment.Length <= 280 && SubComment.Length >= 10 &&
                TempData["currentPostId"] != null)
            {
                int postId = Convert.ToInt32(TempData["currentPostId"]);
                var yorumCheck = _dB.Yorumlar.FirstOrDefault(x =>
                    x.ID == parentContainer && x.PostId == postId);
                if (yorumCheck == null)
                {
                    return MvcHtmlString.Empty;
                }
                AltYorum yeniAltYorum = new AltYorum
                {
                    PostId = postId,
                    CommentTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:sss"),
                    RootCommentId = parentContainer,
                    UserName = commnetOwner.KullaniciAdi,
                    SubDescription = SubComment,
                };
                _dB.AltYorumlar.Add(yeniAltYorum);
                _dB.SaveChanges();
                var adminDeleteSubComment = commnetOwner.Role.Equals("admin")
                    ? $"<a href='#' style='font-size: 14px;margin-top: 45px' class='btn btn-danger' " +
                      $"onclick='altYorumSilBaslat({yeniAltYorum.ID})'>Sil</a>"
                    : "";

                html = $"<div id='altYorum_{yeniAltYorum.ID}' style='display:none' class='media mt-4'>"+
                            "<i class='fa fa-mail-reply fa-5x' style='margin-right: 26px;'></i>"+
                            "<div class='media-body'>"+
                                $"<h5 class='mt-0' style='font-size:17px'>{yeniAltYorum.UserName}</h5>"+
                                $"<p class='meta'><i class='link-spacer'></i>{yeniAltYorum.CommentTime}<i class='link-spacer'></i> Tarihinde yazdı.</p>"+
                                $"<p style = 'font-size: 18px;margin-bottom: 10px' >{yeniAltYorum.SubDescription}</p>"+
                                $"<p class='meta text-danger'><a href = '#' onclick='yanitla({yeniAltYorum.RootCommentId},{yeniAltYorum.ID})' class='text-danger'>" +
                                $"<i class='fa fa-paper-plane-o'></i> Yanıtla</a></p>"+
                            "</div>"+ 
                             adminDeleteSubComment+
                        "</div>";
            }
            TempData["currentPostId"] = Convert.ToInt32(TempData["currentPostId"]);
            html = html.Replace('\'', '"');
            return MvcHtmlString.Create(html);
        }
        [AuthUser]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public MvcHtmlString Yorum(string Description)
        {
            System.Threading.Thread.Sleep(2000);
            Description = HttpUtility.HtmlEncode(Description);
            string html = "";
            Kullanici commentOwner = Session["Kullanici"] as Kullanici;
            if (!String.IsNullOrWhiteSpace(Description) && Description.Length <= 280 && Description.Length >= 10 && TempData["currentPostId"] != null)
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
                       $"<div class='subCommentMenu_{yorum.ID}'></div>"+
                       "</div>" +
                       "</div>";

                
            }
            TempData["currentPostId"] = Convert.ToInt32(TempData["currentPostId"]);

            return MvcHtmlString.Create(html);
        }


    }
}
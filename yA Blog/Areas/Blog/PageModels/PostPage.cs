using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using yA_Blog.Areas.Blog.Models;

namespace yA_Blog.Areas.Blog.PageModels
{
    public class PostPage
    {
        public Haber Post;
        public List<Yorum> Yorumlar;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using yA_Blog.Areas.Blog.Models;

namespace yA_Blog.Areas.Blog.PageModels
{
    public class HomeIndex
    {
        public List<Haber> Haberler;
        public List<Kategori> Kategoriler;
        public List<Tuple<int, int>> YorumCount;

    }
}
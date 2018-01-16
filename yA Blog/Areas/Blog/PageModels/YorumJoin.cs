using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace yA_Blog.Areas.Blog.PageModels
{
    public class YorumJoin
    {
        public int Id { get; set; }
        public string HaberBaslik { get; set; }
        public string Yazar { get; set; }
        public string HaberKategori { get; set; }
        public DateTime YorumZamani { get; set; }
        public string Yorum { get; set; }
    }
}
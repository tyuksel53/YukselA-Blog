using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using yA_Blog.Areas.Blog.Models;
using yA_Blog.Areas.Blog.Models.Managers;

namespace yA_Blog.Areas.Blog.Library
{
    public class CacheHelper
    {
        public static List<Kategori> KategoriGet()
        {
            var result = WebCache.Get("kategori-cache");

            if (result == null)
            {
                DatabaseContext db = new DatabaseContext();
                result = db.Kategoriler.ToList();
                WebCache.Set("kategori-cache", result, 20, true);
            }

            return result;
        }

        public static List<Haber> SonHaberler()
        {
            var result = WebCache.Get("SonHaberler-cache");
            if (result == null)
            {
                DatabaseContext db = new DatabaseContext();
                result = db.Haberler.OrderByDescending(x => x.ID).Take(4).ToList();
                WebCache.Set("SonHaberler-cache",result,20,true);
            }

            return result;
        }

        public static void Remove(string key)
        {
            WebCache.Remove(key:key);
        }

        public static string GetWebSiteName()
        {
            var result = WebCache.Get("websiteConfig-cache");

            if (result == null)
            {
                DatabaseContext _dB = new DatabaseContext();
                result = _dB.Ayarlar.First().WebsiteName;
                WebCache.Set("websiteConfig-cache",result,20,true);
            }

            return result;
        }
    }
}
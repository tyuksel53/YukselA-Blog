using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using yA_Blog.Areas.Blog.Models;

namespace yA_Blog.Areas.Blog.Models.Managers
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Kullanici> Kullanicilar {get;set;}
        public DbSet<Haber> Haberler { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Uploads> Uploads { get; set; }
        public DbSet<Takipciler> Subscribers { get; set; }
        public DbSet<WebSiteConfig> Ayarlar { get; set; }

        public DatabaseContext()
        {
            Database.SetInitializer(new VeriTabaniOlusturucu());
        }
    }
    public class VeriTabaniOlusturucu : CreateDatabaseIfNotExists <DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            WebSiteConfig config = new WebSiteConfig()
            {
                WebsiteName = "Yüksel Algoritma"
            };

            context.Ayarlar.Add(config);

            Kullanici yeni = new Kullanici
            {
                KullaniciAdi = "reistaha",
                Parola = "123123"
            };
            yeni.Parola = Crypto.HashPassword(yeni.Parola);
            yeni.Eposta = "xcvtaha@hotmail.com";
            yeni.ActivateGuid = Guid.NewGuid();
            yeni.PasswordReset = Guid.NewGuid();
            yeni.IsActive = true;
            yeni.Role = "user";
            yeni.ImgUrl = "";

            context.Kullanicilar.Add(yeni);

            for(int i=0;i<5;i++)
            {
                Kategori yeniKategori = new Kategori
                {
                    KategoriIsım = FakeData.NameData.GetCompanyName(),
                    KategoriResim = FakeData.NetworkData.GetDomain()
                };
                context.Kategoriler.Add(yeniKategori);
            }

            context.SaveChanges();
        }
    }
}
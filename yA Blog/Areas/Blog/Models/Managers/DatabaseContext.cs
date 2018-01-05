using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace yA_Blog.Areas.Blog.Models.Managers
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Kullanici> Kullanicilar {get;set;}
        public DbSet<Haber> Haberler { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Uploads> Uploads { get; set; }

        public DatabaseContext()
        {
            Database.SetInitializer(new VeriTabaniOlusturucu());
        }
    }
    public class VeriTabaniOlusturucu : CreateDatabaseIfNotExists <DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            Kullanici yeni = new Kullanici
            {
                KullaniciAdi = "reistaha",
                Parola = "123123"
            };
            yeni.Parola = Crypto.HashPassword(yeni.Parola);
            yeni.Eposta = "xcvtaha@hotmail.com";

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
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

        public DatabaseContext()
        {
            Database.SetInitializer(new VeriTabaniOlusturucu());
        }
    }
    public class VeriTabaniOlusturucu : CreateDatabaseIfNotExists <DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            Kullanici yeni = new Kullanici();
            yeni.KullaniciAdi = "reistaha";
            yeni.Parola = "123123";
            yeni.Parola = Crypto.HashPassword(yeni.Parola);
            yeni.Eposta = "xcvtaha@hotmail.com";

            context.Kullanicilar.Add(yeni);

            for(int i=0;i<5;i++)
            {
                Kategori yeniKategori = new Kategori();
                yeniKategori.KategoriIsım = FakeData.NameData.GetCompanyName();
                yeniKategori.KategoriResim = FakeData.NetworkData.GetDomain();
                context.Kategoriler.Add(yeniKategori);
            }

            context.SaveChanges();
        }
    }
}
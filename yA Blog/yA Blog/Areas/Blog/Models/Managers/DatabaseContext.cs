using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace yA_Blog.Areas.Blog.Models.Managers
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Kullanici> Kullanicilar {get;set;}

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
            yeni.Eposta = "xcvtaha@hotmail.com";

            context.Kullanicilar.Add(yeni);

            context.SaveChanges();
        }
    }
}
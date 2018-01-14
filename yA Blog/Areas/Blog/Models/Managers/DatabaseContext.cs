﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
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
        public DbSet<Yorum> Yorumlar { get; set; }

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

            Kullanici user = new Kullanici
            {
                KullaniciAdi = "admin",
                Parola = Crypto.HashPassword("1234"),
                Eposta = "xcvtaha@hotmail.com",
                ActivateGuid = Guid.NewGuid(),
                PasswordReset = Guid.NewGuid(),
                IsActive = true,
                Role = "admin",
                ImgUrl = "../../Areas/Blog/Uploads/admin.svg"
            };

            context.Kullanicilar.Add(user);

            Kullanici admin = new Kullanici
            {
                KullaniciAdi = "user",
                Parola = Crypto.HashPassword("1234"),
                Eposta = "xcvtaha@hotmail.com",
                ActivateGuid = Guid.NewGuid(),
                PasswordReset = Guid.NewGuid(),
                IsActive = true,
                Role = "admin",
                ImgUrl = ""
            };

            context.Kullanicilar.Add(admin);
            Random random = new Random();
            for (int i=0;i<10;i++)
            {
                string path = "../../Areas/Blog/Uploads/img/";
                Kategori yeniKategori = new Kategori
                {
                    KategoriIsım = FakeData.NameData.GetCompanyName(),
                    KategoriResim =  $"{path}cover-{i+1}.jpg"
                };
                context.Kategoriler.Add(yeniKategori);
            }

            context.SaveChanges();
            
            for (int i = 0; i < 6; i++)
            {
                int randomInt = random.Next(1, 5);
                Haber post = new Haber()
                {
                    HaberBaslik = FakeData.TextData.GetSentence(),
                    HaberIcerik = "<p>"+FakeData.TextData.GetSentences(20)+"</p>",
                    HaberOzet = FakeData.TextData.GetSentence(),
                    HaberResimUrl = "../../Areas/Blog/Uploads/img/default-single-hero-with-sidebar.jpg",
                    Kategorisi = context.Kategoriler.FirstOrDefault(x=> x.ID == randomInt ),
                    HaberYayinlamaTarih = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy"),
                    Tags = "C#,Mvc,EntityFramework,Linq",
                    Yazar = context.Kullanicilar.FirstOrDefault(x => x.ID == 1)
                };

                context.Haberler.Add(post);
            }

            context.SaveChanges();

            for (int i = 0; i < 20; i++)
            {
                int randomInt = random.Next(1, 6);
                Yorum yeniYorum = new Yorum
                {
                    PostId = context.Haberler.FirstOrDefault(x => x.ID == randomInt).ID,
                    CommentTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
                    Description = FakeData.TextData.GetSentences(2),
                    UserName = context.Kullanicilar.FirstOrDefault().KullaniciAdi
                };
                context.Yorumlar.Add(yeniYorum);
            }
            string[] uploaddedFiles = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Areas/Blog/Uploads/img"))
            .Select(Path.GetFileName)
            .ToArray();

            foreach (var item in uploaddedFiles)
            {
                Uploads upload = new Uploads
                {
                    DosyaAdı = Path.GetFileName(item),
                    DosyaUzantisi = Path.GetExtension(item),
                    YuklenmeTarihi = DateTime.Now.ToString("dd-MM-yyyy"),
                    DosyaYolu = HttpContext.Current.Server.MapPath("~/Areas/Blog/Uploads/img/") + item
                };
                context.Uploads.Add(upload);
            }

            context.SaveChanges();

        }
    }
}
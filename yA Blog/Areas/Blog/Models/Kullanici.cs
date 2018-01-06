using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.DynamicData;

namespace yA_Blog.Areas.Blog.Models
{
    [TableName("Kullanicilar")]
    public class Kullanici
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID{ get; set; }

        [Required,DisplayName("Kullanici Adi"),MinLength(4,ErrorMessage = "{0}nız minimum {1} karakter olmalıdır"),
            MaxLength(65,ErrorMessage = "{0}nız {1} karakterden uzun olamaz" )]
        public string KullaniciAdi { get; set; }

        [Required,DisplayName("Şifre"),MinLength(4,ErrorMessage = "{0}nız minimum {1} karakter olmalıdır")]
        public string Parola { get; set; }

        [DisplayName("Eposta Adresi"),EmailAddress(),Required]
        public string Eposta { get; set; }

        public string ImgUrl { get; set; }

        public string Role;

        public Guid ActiveGuid;

        public bool IsActive;

    }
}
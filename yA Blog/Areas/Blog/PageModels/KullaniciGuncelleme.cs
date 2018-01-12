using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace yA_Blog.Areas.Blog.PageModels
{
    public class KullaniciGuncelleme
    {
        [Required, DisplayName("Suanki sifrenizi girin"), DataType(DataType.Password), 
         MinLength(4, ErrorMessage = "{0}nız minimum {1} karakter olmalıdır")]
        public string EskiParola { get; set; }

        [Required, DisplayName("Yeni Şifreniz"), DataType(DataType.Password), 
         MinLength(4, ErrorMessage = "{0}nız minimum {1} karakter olmalıdır")]
        public string YeniParola { get; set; }

        [Required,DisplayName("Yeni Sifreniz Tekrar"), DataType(DataType.Password), 
         MinLength(4, ErrorMessage = "{0}nız minimum {1} karakter olmalıdır")]
        [Compare(nameof(YeniParola), ErrorMessage = "Şifreler eslesmiyor")]
        public string YeniParolaOnay { get; set; }

        [DisplayName("Yeni Eposta Adresiniz"),Required(ErrorMessage = "Bu alan gereklidir")]
        [EmailAddress(ErrorMessage = "Lutfen duzgun formatta mail adresi girin.")]
        public string Email { get; set; }

        [DisplayName("Yeni Eposta Adresi Tekrar"),Required(ErrorMessage = "Bu alan gereklidir")]
        [EmailAddress(ErrorMessage = "Lutfen duzgun formatta mail adresi girin.")]
        [Compare(nameof(Email), ErrorMessage = "Girdiginiz eposta adresleri eslesmiyor")]
        public string EmailOnay { get; set; }
    }
}
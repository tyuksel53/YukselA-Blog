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
    [TableName("Haberler")]
    public class Haber
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required,DisplayName("Haber Baslığı"), 
         MinLength(10, ErrorMessage = "{0} minimum {1} karakter olmalıdır"),
         MaxLength(280, ErrorMessage = "{0}niz maximum {1} karakteri geçmemelidir")]
        public string HaberBaslik { get; set; }

        [Required ,DisplayName("Haber Özeti"), 
         MinLength(4, ErrorMessage = "{0}niz minimum {1} karakter olmalıdır"),
         MaxLength(400, ErrorMessage ="{0}niz maximum {1} karakteri geçmemelidir")]
        public string HaberOzet { get; set; }

        [Required, DisplayName("Haber İçeriği"), MinLength(100, ErrorMessage = "{0}niz minimum {1} karakter olmalıdır")]
        public string HaberIcerik { get; set; }


        public List<string> Tags { get; set; }

        [Required]
        public string HaberResimUrl { get; set; }

        [Required]
        public virtual Kullanici Yazar {get;set;}
    }
}
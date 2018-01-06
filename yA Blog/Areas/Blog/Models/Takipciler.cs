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
    [TableName("Takipciler")]
    public class Takipciler
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required(ErrorMessage = "Eposta Adresi Gereklidir"),DisplayName("Eposta Adresiniz"), EmailAddress()]
        public string Email { get; set; }

        public string KayitTarihi { get; set; }

        public Guid DelToken;
    }
}
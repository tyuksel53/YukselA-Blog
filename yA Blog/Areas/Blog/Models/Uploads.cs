using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace yA_Blog.Areas.Blog.Models
{
    public class Uploads
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string DosyaAdı { get; set; }

        public DateTime YuklenmeTarihi { get; set; }

        public string DosyaYolu { get; set; }

        public string DosyaUzantisi { get; set; }
    }
}
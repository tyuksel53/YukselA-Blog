using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.DynamicData;

namespace yA_Blog.Areas.Blog.Models
{
    [TableName("Kategoriler")]
    public class Kategori
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DisplayName("Kategori İsmi"),Required]
        public string KategoriIsım { get; set; }

        [DisplayName("Kategori Resmi"),Required]
        public string KategoriResim { get; set; }
    }
}
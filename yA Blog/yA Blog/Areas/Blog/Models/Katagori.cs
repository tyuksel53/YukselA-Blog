using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.DynamicData;

namespace yA_Blog.Areas.Blog.Models
{
    [TableName("Katagoriler")]
    public class Katagori
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DisplayName("Katagori İsmi"),Required]
        public string KatagoriIsım { get; set; }

        [DisplayName("Katagori Resmi"),Required]
        public string KatagoriResim { get; set; }
    }
}
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
    [TableName("Ayarlar")]
    public class WebSiteConfig
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [DisplayName("Websitenizin Adı")]
        public string WebsiteName { get; set; }

        [Required]
        [DisplayName("Websitenizin Açıklaması")]
        [DataType(DataType.MultilineText)]
        public string WebsiteInfo { get; set; }

        [Required]
        [DisplayName("Takipcilere bildirim gitsin mi")]
        public bool Subscribers { get; set; }

        [Required]
        [DisplayName("Ana Sayfa Kapak Fotoğrafı")]
        public string IndexImg { get; set; }

    }
}
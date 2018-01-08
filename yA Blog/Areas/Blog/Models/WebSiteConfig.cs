using System;
using System.Collections.Generic;
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

        public string WebsiteName { get; set; }

    }
}
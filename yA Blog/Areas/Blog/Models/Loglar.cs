using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace yA_Blog.Areas.Blog.Models
{
    public class Loglar
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("İşlem Tarihi"),DataType(DataType.DateTime),Required]
        public DateTime Tarih { get; set; }

        [Required]
        public string KullaniciAdi  { get; set; }  

        [Required]
        public string KullaniciIp { get; set; }

        [Required,StringLength(100),DisplayName("Action")]
        public string ActionName { get; set; }

        [Required,StringLength(100),DisplayName("Controller")]
        public string ControllerName { get; set; }

        [Required,StringLength(100),DisplayName("Bilgi")]
        public string Bilgi { get; set; }
    }
}
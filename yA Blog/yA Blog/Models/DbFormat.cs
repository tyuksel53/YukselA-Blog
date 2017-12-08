using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace yA_Blog.Models
{
    public class DbFormat
    {
        [DisplayName("Server Adı:"),Required(ErrorMessage = "Bu alan gereklidir")  ] 
        public string Servername         { get; set; }

        [DisplayName("Veritabanı Adı:"),Required(ErrorMessage ="Bu alan gereklidir")]
        public string DbName             { get; set; }

        [DisplayName("Windows Authentication")]
        public bool   AuthenticationMode { get; set; }

        [DisplayName("Kullanıcı Adi:")]
        public string Username           { get; set; }

        [DisplayName("Parolanız"),DataType(DataType.Password)]
        public string Password           { get; set; }
    }
}
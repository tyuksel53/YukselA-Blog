using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace yA_Blog.Areas.Blog.Library
{
    public class File
    {
        [Required(ErrorMessage = "Lütfen dosya seçin")]
        [Display(Name = "Dosya Seçin")]
        public HttpPostedFileBase[] files { get; set; }
    }
}
﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.DynamicData;

namespace yA_Blog.Areas.Blog.Models
{
    [TableName("Alt Yorumlar")]
    public class AltYorum
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string UserName { get; set; }

        [Required(ErrorMessage = "Bu alan gereklidir"), DisplayName("Yorumuz"),
         MinLength(10, ErrorMessage = "{0} minimum {1} karakter olmalıdır"),
         MaxLength(280, ErrorMessage = "{0}niz maximum {1} karakteri geçmemelidir"),
         DataType(DataType.MultilineText)]
        public string SubDescription { get; set; }

        public string CommentTime { get; set; }

        public int PostId { get; set; }

        public int RootCommentId { get; set; }


    }
}
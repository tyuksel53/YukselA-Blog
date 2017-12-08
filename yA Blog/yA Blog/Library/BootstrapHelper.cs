using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace yA_Blog.Library
{
    public static class BootstrapHelper
    {
        public static MvcHtmlString Modal(this HtmlHelper helper, string modalID, string ModalIHeader, string ModalDescription)
        {
            string modal = string.Format("" +
             "<div class='modal fade' id='{0}' role='dialog'>" +
                "<div class='modal-dialog'>" +
                  "<div class='modal-content'>" +
                    "<div class='modal-header'>" +
                      "<h4 class='modal-title'>{1}</h4>" +
                      "<button type = 'button' class='close' data-dismiss='modal'>&times;</button>" +
                    "</div>" +
                    "<div class='modal-body'>" +
                      "<p>{2}</p>" +
                    "</div>" +
                    "<div class='modal-footer'>" +
                      "<button type = 'button' class='btn btn-default' data-dismiss='modal'>Close</button>" +
                    "</div>" +
                  "</div>" +
                "</div>" +
              "</div>", modalID, ModalIHeader, ModalDescription);
            return MvcHtmlString.Create(modal);
        }
    }
}
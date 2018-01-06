using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace yA_Blog.Areas.Blog.Library
{
    public class Portal
    {
        public static bool IsValidMail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
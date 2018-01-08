using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using Newtonsoft.Json;
using yA_Blog.Library;

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

        public static bool SendMail(string body,List<string> to,string subject,bool isHtml= true)
        {
            bool result = false;

            try
            {
                var message = new MailMessage();
                message.From = new MailAddress(WebConfigGet<string>("mailUser"));

                to.ForEach(x => { message.To.Add(new MailAddress(x)); });

                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = isHtml;

                using (var smtp = new SmtpClient(WebConfigGet<string>("mailHost"),
                    Portal.WebConfigGet<int>("mailPort")))
                {
                    smtp.EnableSsl = true;
                    smtp.Credentials = new NetworkCredential(WebConfigGet<string>("mailUser"),
                        WebConfigGet<string>("mailPass"));
                    smtp.Send(message);
                    result = true;
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public static bool SendMail(string body, string to, string subject, bool isHtml = true)
        {
            return SendMail(body, new List<string> {to}, subject, isHtml);
        }

        public static T WebConfigGet<T>(string key)
        {
            return (T) Convert.ChangeType(ConfigurationManager.AppSettings[key],typeof(T));
        }

        public static bool CheckCaptcha(string response)
        {

            string secret = WebConfigurationManager.AppSettings["recaptcha_privatekey"].ToString();

            var client = new WebClient();
            var reply =
                client.DownloadString(
                    string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

            if (!captchaResponse.Success)
            {
                return false;

            }
            else
            {
                return true;
            }
        }
    }
}
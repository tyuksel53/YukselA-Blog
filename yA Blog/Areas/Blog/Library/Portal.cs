using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.Mvc;
using Newtonsoft.Json;
using yA_Blog.Areas.Blog.Models;
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

            string secret = WebConfigGet<string>("recaptcha_privatekey");

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
        public static List<SelectListItem> Katagorileri_Getir(int id)
        {
            List<SelectListItem> kisilerListe =
            (from s in CacheHelper.KategoriGet()
                select new SelectListItem()
                {
                    Text = s.KategoriIsım,
                    Value = s.ID.ToString(),
                    Selected = s.ID == id ? true : false
                }
            ).ToList();

            return kisilerListe;
        }

        public static List<SelectListItem> Roller()
        {
            List<SelectListItem> roller = new List<SelectListItem>();

            roller.Add(new SelectListItem()
            {
                Text = "user",
                Value = 1.ToString(),
                Selected = true

            });

            roller.Add(new SelectListItem()
            {
                Text = "admin",
                Value = 2.ToString()
            });

            return roller;
        }

        public static void AktivasyonMailGonder(Kullanici model)
        {
            string siteURL = Portal.WebConfigGet<string>("SiteRootUri");

            string activateURL = $"{siteURL}/Blog/Home/UserActivate?activateId={model.ActivateGuid}";

            string activateLink = $"<a href='{activateURL}' target='_blank' > tıklayınız.</a>.";

            string siteName = CacheHelper.GetWebSiteName();

            string body = $"Merhaba {model.KullaniciAdi},<br/><br/> Hesabınızı Aktifleştirmek için {activateLink}";

            Portal.SendMail(body, model.Eposta, (siteName + " Hesabınızı Aktifleştirme"));
        }
    }
}
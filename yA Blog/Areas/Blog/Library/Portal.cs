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
                    WebConfigGet<int>("mailPort")))
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
                // ignored
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
                    Text = s.KategoriIsim,
                    Value = s.ID.ToString(),
                    Selected = s.ID == id ? true : false
                }
            ).ToList();

            return kisilerListe;
        }

        public static List<SelectListItem> Roller()
        {
            List<SelectListItem> roller = new List<SelectListItem>
            {
                new SelectListItem()
                {
                    Text = "user",
                    Value = 1.ToString(),
                    Selected = true
                },
                new SelectListItem()
                {
                    Text = "admin",
                    Value = 2.ToString()
                }
            };



            return roller;
        }

        public static void AktivasyonMailGonder(Kullanici model)
        {
            string siteUrl = WebConfigGet<string>("SiteRootUri");

            string activateUrl = $"{siteUrl}/Home/UserActivate?activateId={model.ActivateGuid}";

            string activateLink = $"<a href='{activateUrl}' target='_blank' > tıklayınız.</a>.";

            string siteName = CacheHelper.GetWebSiteName();

            string body = $"Merhaba {model.KullaniciAdi},<br/><br/> Hesabınızı Aktifleştirmek için {activateLink}";

            SendMail(body, model.Eposta, (siteName + " Hesabınızı Aktifleştirme"));
        }

        public static void KullaniciSifreReset(Kullanici model)
        {
            string siteUrl = WebConfigGet<string>("SiteRootUri");

            string activateUrl = $"{siteUrl}/Home/PasswordReset?reset={model.PasswordReset}";

            string activateLink = $"<a href='{activateUrl}' target='_blank' > tıklayınız.</a>.";

            string siteName = CacheHelper.GetWebSiteName();

            string body = $"Merhaba {model.KullaniciAdi},<br/><br/> Şifrenizi resetlemek için {activateLink}";

            SendMail(body, model.Eposta, (siteName + " Şifrenizi resetleme"));
        }

        public static void TakipciBildirim(Takipciler takipci,string haberBaslik)
        {
            string siteUrl = WebConfigGet<string>("SiteRootUri");

            string[] split = haberBaslik.Split('-');

            var haberUrl = siteUrl + "/Post/Haber/" + SeoUrl(split[0]) + "-" + split[1];

            string deActivateUrl = $"{siteUrl}/Home/UnSubscribe?deActive={takipci.DelToken}";

            deActivateUrl = $"<a href='{deActivateUrl}' target='_blank' > tıklayınız.</a>";

            haberUrl = $"<a href='{haberUrl}' target='_blank' > tıklayınız.</a>";

            string siteName = CacheHelper.GetWebSiteName();

            string body = $"Merhabalar sayın takipcimiz,<br/><br/>Yeni içeriğimiz, <b><i>{split[0]}</i></b>" +
                          $", isimli habere bakmak için {haberUrl}<br/><br/>" +
                          $"Takipçilikten çıkmak için {deActivateUrl}";

            SendMail(body, takipci.Email, (siteName + " Bildirimler"));
        }


        public static string SeoUrl(string text)
        {
            try
            {
                string strReturn = text.Trim();
                strReturn = strReturn.Replace("ğ", "g");
                strReturn = strReturn.Replace("Ğ", "G");
                strReturn = strReturn.Replace("ü", "u");
                strReturn = strReturn.Replace("Ü", "U");
                strReturn = strReturn.Replace("ş", "s");
                strReturn = strReturn.Replace("Ş", "S");
                strReturn = strReturn.Replace("ı", "i");
                strReturn = strReturn.Replace("İ", "I");
                strReturn = strReturn.Replace("ö", "o");
                strReturn = strReturn.Replace("Ö", "O");
                strReturn = strReturn.Replace("ç", "c");
                strReturn = strReturn.Replace("Ç", "C");
                strReturn = strReturn.Replace("-", "+");
                strReturn = strReturn.Replace(" ", "+");
                strReturn = strReturn.Trim();
                strReturn = new System.Text.RegularExpressions.Regex("[^a-zA-Z0-9+]").Replace(strReturn, "");
                strReturn = strReturn.Trim();
                strReturn = strReturn.Replace("+", "-");
                return strReturn;
            }
            catch (Exception ex)
            {
                return "olmadi";
            }

        }
    }
}
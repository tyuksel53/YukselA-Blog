using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace yA_Blog.Controllers
{
    public class InstallController : Controller
    {
        // GET: Install
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Install()
        {
            return View();
        }

        public JsonResult VeriTabanı_Control(int veritabanıType,string veritabani_adresi,string veritabani_port,string veritabani_adi,string veritabani_kullaniciAdi,string veritabani_sifre)
        {

            System.Configuration.Configuration Config1 = WebConfigurationManager.OpenWebConfiguration("~");
            ConnectionStringsSection conSetting = (ConnectionStringsSection)Config1.GetSection("connectionStrings");
            ConnectionStringSettings StringSettings = new ConnectionStringSettings("conn", "Server=" + veritabani_adresi + ";Database=" + veritabani_adi + ";uid=" + veritabani_kullaniciAdi + ";Password=" + veritabani_sifre + ";", "MySql.Data.MySqlClient");

            conSetting.ConnectionStrings.Remove(StringSettings);
            conSetting.ConnectionStrings.Add(StringSettings);
            Config1.Save(ConfigurationSaveMode.Modified);

            var connection =
             System.Configuration.ConfigurationManager.
             ConnectionStrings["conn"].ConnectionString;

            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(connection);
                conn.Open();
                return Json(conn, JsonRequestBehavior.AllowGet);
            }
            catch (Exception a_ex)
            {
                
            }

            bool deneme = true;
            return Json(deneme, JsonRequestBehavior.AllowGet);
        }
    }
}
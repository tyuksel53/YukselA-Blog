using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using yA_Blog.Models;

namespace yA_Blog.Controllers
{
    public class InstallController : Controller
    {
        // GET: Install
        public ActionResult Install()
        {
            return View();
        }

        public JsonResult CheckDb_Working()
        {
            try
            {
              var connection =
              System.Configuration.ConfigurationManager.
              ConnectionStrings["conn"].ConnectionString;
              SqlConnection con = new SqlConnection(connection);
              con.Open();
                con.Close();
              return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        public PartialViewResult addConnection_String(DbFormat model)
        {
            System.Threading.Thread.Sleep(1000);
            if (ModelState.IsValid)
            {
                if(!model.AuthenticationMode)
                {
                    if(String.IsNullOrEmpty(model.Username))
                    {
                        ModelState.AddModelError("Username", "Kullanıcı adi boş geçilemez");
                        return PartialView("_InstallPartialView", model);
                    }

                    if(String.IsNullOrEmpty(model.Password))
                    {
                        ModelState.AddModelError("Password", "Parola boş geçilemez");
                        return PartialView("_InstallPartialView", model);
                    }

                    System.Configuration.Configuration Config1 = WebConfigurationManager.OpenWebConfiguration("~");
                    ConnectionStringsSection conSetting = (ConnectionStringsSection)Config1.GetSection("connectionStrings");

                    ConnectionStringSettings StringSettings = new ConnectionStringSettings("conn", "Data Source=" + model.Servername +
                    ";Initial Catalog=" + model.DbName + ";User Id=" +
                    model.Username + ";Password=" + model.Password + ";", "System.Data.SqlClient");

                    conSetting.ConnectionStrings.Remove(StringSettings);
                    conSetting.ConnectionStrings.Add(StringSettings);
                    Config1.Save(ConfigurationSaveMode.Modified);
                    
                }
                else // Windows AutenticationMode
                {
                    System.Configuration.Configuration Config1 = WebConfigurationManager.OpenWebConfiguration("~");
                    ConnectionStringsSection conSetting = (ConnectionStringsSection)Config1.GetSection("connectionStrings");

                    ConnectionStringSettings StringSettings = new ConnectionStringSettings("conn", "Data Source=" + model.Servername +
                    ";Initial Catalog=" + model.DbName + ";Integrated Security=True;", "System.Data.SqlClient");

                    conSetting.ConnectionStrings.Remove(StringSettings);
                    conSetting.ConnectionStrings.Add(StringSettings);
                    Config1.Save(ConfigurationSaveMode.Modified);
                }

                return PartialView("_InstallPartialView", model);
            }
            else
            {
                return PartialView("_InstallPartialView", model);
            }
        }


    }
}
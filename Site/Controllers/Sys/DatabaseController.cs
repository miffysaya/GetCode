using AgileFramework.Web;
using AgileFramework.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.GetCode.Site.Common;
using Da = WebProject.GetCode.DataAccess;
using En = WebProject.GetCode.Entity;
using Mo = WebProject.GetCode.Site.Models;

namespace WebProject.GetCode.Site.Controllers.Sys
{
    [IdentityAuthorize]
    public class DatabaseController : Controller
    {
        // GET: Database
        public ActionResult GetAllDatabases()
        {
            var actionResult = default(AgileJsonResult);

            try
            {
                var databases = Da.Sys.GetAllDatabases(Config.ConnectionString_Read);

                actionResult = new AgileJsonResult()
                {
                    Content = AgileJson.ToJson(new { result = databases })
                };
            }
            catch { }

            return actionResult;
        }

        public ActionResult GetAllTables()
        {
            var model = new Mo.SysDatabase();

            UpdateModel(model);

            var actionResult = default(AgileJsonResult);

            try
            {
                var tables = Da.Sys.GetAllTables(Config.ConnectionString_Read, model.DbName);

                actionResult = new AgileJsonResult()
                {
                    Content = AgileJson.ToJson(new
                    {
                        result = tables
                    })
                };
            }
            catch { }

            return actionResult;
        }

        public ActionResult GenerateEntity()
        {
            var model = new Mo.SysDatabase();

            UpdateModel(model);

            var actionResult = default(AgileJsonResult);

            try
            {
                dynamic wherePart = new ExpandoObject();

                wherePart.DBName = model.DbName;

                wherePart.ID = model.TableID;

                var tables = Da.Sys.GetAllTables(Config.ConnectionString_Read, wherePart);

                actionResult = new AgileJsonResult()
                {
                    Content = AgileJson.ToJson(new
                    {
                        result = tables
                    })
                };
            }
            catch { }

            return actionResult;
        }
    }
}
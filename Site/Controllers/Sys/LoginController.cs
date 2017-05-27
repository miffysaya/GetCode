using AgileFramework.Business;
using AgileFramework.Web;
using AgileFramework.Web.Mvc;
using System.Web.Mvc;
using WebProject.GetCode.Site.Common;
using Mo = WebProject.GetCode.Site.Models;

namespace WebProject.GetCode.Site.Controllers.Sys
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return Redirect("http://www.baidu.com");
        }
        public ActionResult Enter()
        {
            var actionResult = default(AgileJsonResult);

            var inModel = new Mo.SysUser();

            try
            {
                UpdateModel(inModel);

                var url = Config.PDAUrl;

                var method = AgilePDAMethod.Login;

                var parameter = $"username={inModel.UserName}&password={inModel.Password}";

                var entity = AgilePDASoap.Post(url, method, parameter);

                actionResult.Content = AgileJson.ToJson(entity);

                if (entity.Status == "1")
                {
                    Identity.Login(inModel);
                }
            }
            catch
            {

            }

            return actionResult;
        }
    }
}
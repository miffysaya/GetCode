using AgileFramework.Web.Mvc;
using System.Web.Mvc;

namespace WebProject.GetCode.Site
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AgileHandleErrorAttribute() { Master = null, View = "404页" });
        }
    }
}

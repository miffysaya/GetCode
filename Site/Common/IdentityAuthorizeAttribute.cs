using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebProject.GetCode.Site.Common
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class IdentityAuthorizeAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var requestHeaders = filterContext.HttpContext.Request.Headers;

            var origin = requestHeaders["Origin"];

            if (origin != null)
            {
                if (Config.AccessControlAllowOrigin.Contains(origin))
                {
                    filterContext.HttpContext.Response.Headers.Remove("Access-Control-Allow-Origin");
                    filterContext.HttpContext.Response.Headers.Set("Access-Control-Allow-Origin", origin);
                }
            }
            //try
            //{
            //    if (!Identity.IsAuthenticated)
            //    {
            //        HttpContext.Current.Response.Clear();
            //        HttpContext.Current.Response.Write("<script>window.top.location='" + Config.LoginUrl + "'</script>");
            //        HttpContext.Current.Response.End();
            //    }
            //    base.OnActionExecuting(filterContext);
            //}
            //catch (Exception exception)
            //{
            //    HttpContext.Current.Response.Clear();
            //    HttpContext.Current.Response.Write(exception.Message);
            //    HttpContext.Current.Response.End();
            //}
            base.OnActionExecuting(filterContext);
        }
    }
}
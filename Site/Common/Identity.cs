using AgileFramework.Security;
using AgileFramework.Web;
using System;
using System.Linq;
using System.Web;
using Mo = WebProject.GetCode.Site.Models;

namespace WebProject.GetCode.Site.Common
{
    /// <summary>
    /// 权限类
    /// </summary>
    public class Identity
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public string LoginTime { get; set; }

        /// <summary>
        /// 用户标识
        /// </summary>
        public static Identity User
        {
            get
            {
                try
                {
                    var key = string.Format("{0}.Identity", Config.CookiePrefix);
                    var cookies = HttpContext.Current.Request.Cookies;
                    if (cookies != null && cookies.AllKeys.Contains(key))
                    {
                        var json = cookies[key].Value;
                        if (string.IsNullOrEmpty(json))
                        {
                            return null;
                        }
                        var user = AgileJson.FromJson<Identity>(AgileDES.Decrypt(json, Config.SecurityKey));

                        if (DateTime.Parse(user.LoginTime) < DateTime.Now.AddDays(-1))
                        {
                            return null;
                        }

                        return user;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 是否验证
        /// </summary>
        public static bool IsAuthenticated
        {
            get
            {
                try
                {
                    var key = string.Format("{0}.Identity", Config.CookiePrefix);
                    var cookies = HttpContext.Current.Request.Cookies;
                    if (cookies != null && cookies.AllKeys.Contains(key))
                    {
                        var json = cookies[key].Value;
                        if (string.IsNullOrEmpty(json))
                        {
                            return false;
                        }
                        var user = AgileJson.FromJson<Identity>(AgileDES.Decrypt(json, Config.SecurityKey));

                        if (DateTime.Parse(user.LoginTime) < DateTime.Now.AddDays(-1))
                        {
                            return false;
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="email">用户标识</param>
        public static void Login(Mo.SysUser sysUser)
        {
            var cookies = HttpContext.Current.Response.Cookies;
            var user = new Identity { UserName = sysUser.UserName, Password = sysUser.Password, LoginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
            var json = AgileJson.ToJson(user);
            var key = string.Format("{0}.Identity", Config.CookiePrefix);
            cookies[key].Value = AgileDES.Encrypt(json, Config.SecurityKey);
        }

        /// <summary>
        /// 退出
        /// </summary>
        public static void Logout()
        {
            foreach (string key in HttpContext.Current.Request.Cookies.AllKeys)
            {
                HttpContext.Current.Response.Cookies[key].Value = null;
                HttpContext.Current.Response.Cookies[key].Expires = DateTime.Now.AddDays(-1);
            }

            //将单点登录系统的用户也注销掉
            HttpContext.Current.Response.Write("<script>window.top.location='" + Config.LoginUrl + "'</script>");
        }
    }
}
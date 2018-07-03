﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Known.Providers;

namespace Known.Web
{
    /// <summary>
    /// 应用程序全局事件。
    /// </summary>
    public class Global : System.Web.HttpApplication
    {
        /// <summary>
        /// 应用程序启动时执行事件。
        /// </summary>
        /// <param name="sender">事件触发者。</param>
        /// <param name="e">事件参数。</param>
        protected void Application_Start(object sender, EventArgs e)
        {
            Environment.CurrentDirectory = HttpRuntime.AppDomainAppPath;
            ProviderConfig.RegisterProviders();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        /// <summary>
        /// 应用程序错误捕获事件。
        /// </summary>
        /// <param name="sender">事件触发者。</param>
        /// <param name="e">事件参数。</param>
        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex != null)
            {
                Mail.Send("Web程序发生异常", ex);
            }
        }
    }
}
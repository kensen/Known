﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Web;
using Known.Extensions;
using Known.Log;

namespace Known.Web.Mvc
{
    /// <summary>
    /// Http模块。
    /// </summary>
    public class HttpModule: IHttpModule
    {
        private HttpContext context;

        /// <summary>
        /// 初始化Http应用程序。
        /// </summary>
        /// <param name="context">应用程序。</param>
        public void Init(HttpApplication context)
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            WebApp.Init();
            context.BeginRequest += Context_BeginRequest;
            context.PostMapRequestHandler += Context_PostMapRequestHandler;
            context.AuthorizeRequest += Context_AuthorizeRequest;
            context.AcquireRequestState += Context_AcquireRequestState;
            context.EndRequest += Context_EndRequest;
            context.Error += Context_Error;
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            context = app.Context;

            var request = context.Request;
            var checker = new XSSChecker(request);
            if (request.Cookies != null)
            {
                if (checker.CheckCookieData())
                    ErrorResult("您提交的数据有恶意字符！");
            }
            if (request.UrlReferrer != null)
            {
                if (checker.CheckUrlReferer())
                    ErrorResult("您提交的数据有恶意字符！");
            }
            if (request.RequestType.ToUpper() == "POST")
            {
                if (checker.CheckPostData())
                    ErrorResult("您提交的数据有恶意字符！");
            }
            if (request.RequestType.ToUpper() == "GET")
            {
                if (checker.CheckGetData())
                    ErrorResult("您提交的数据有恶意字符！");
            }
        }

        private void Context_PostMapRequestHandler(object sender, EventArgs e)
        {
            context.Handler = SessionHandler.Instance;
        }

        private void Context_AuthorizeRequest(object sender, EventArgs e)
        {
            var user = context.User;
        }

        private void Context_AcquireRequestState(object sender, EventArgs e)
        {
            context.Response.HeaderEncoding = Encoding.Default;
            context.Response.ContentEncoding = Encoding.Default;

            var url = context.Request.Url.LocalPath.Trim('/');
            InvokeAction(url);
        }

        private void Context_EndRequest(object sender, EventArgs e)
        {
            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-AspNet-Version");
            context.Response.Headers.Remove("X-AspNetMvc-Version");
        }

        private void Context_Error(object sender, EventArgs e)
        {
            var user = context.User.Identity.Name;
            var url = context.Request.Url;
            var error = context.Error.ToString();
            LogHelper.Error($"{user} - {url} - {error}");
        }

        private void ErrorResult(string message)
        {
            context.Response.Write(new { ok = false, message }.ToJson());
            context.Response.End();
        }

        private void InvokeAction(string url)
        {
            if (url.Contains("static"))
                return;

            if (string.IsNullOrWhiteSpace(url))
                url = "Home/Index";

            var action = WebApp.GetAction(url);
            if (action.Controller == null || action.Method == null)
            {
                context.Response.Write($"{url}不存在！");
                return;
            }

            InvokeAction(action);
        }

        private void InvokeAction(ActionInfo action)
        {
            try
            {
                var queries = HttpUtility.ParseQueryString(context.Request.Url.Query);
                if (queries != null && queries.Count > 0)
                    action.Datas = queries.ToDictionary();
                
                var obj = Activator.CreateInstance(action.Controller) as Controller;
                obj.Context = new ControllerContext(context, action);

                var result = InvokeAction(obj, action);
                if (result != null)
                {
                    if (result is ActionResult ar)
                        ar.Execute();
                    else
                        context.Response.Write(result.ToString());
                }
            }
            catch (ThreadAbortException)
            {
                //Reponse.End提前终止线程
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.Message);
            }
        }

        private object InvokeAction(Controller obj, ActionInfo action)
        {
            var method = action.Method;
            var datas = action.Datas;
            var parameterInfos = method.GetParameters();
            if (parameterInfos == null || parameterInfos.Length == 0)
                return method.Invoke(obj, null);

            var parameters = new List<object>();
            if (datas == null || datas.Count == 0)
                throw new Exception($"{method.Name}参数不能为空！");

            foreach (var item in parameterInfos)
            {
                if (datas.ContainsKey(item.Name))
                    parameters.Add(datas[item.Name]);
                else
                    parameters.Add(null);
            }

            return method.Invoke(obj, parameters.ToArray());
        }
    }
}

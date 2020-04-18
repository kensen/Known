﻿using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Known.Web;

namespace Known.Core
{
    public class HomeController : Web.ControllerBase
    {
        #region View
        [AllowAnonymous, Route("login")]
        public ActionResult Login()
        {
            return ViewResult();
        }

        public ActionResult Index()
        {
            return ViewResult();
        }

        public ActionResult Welcome()
        {
            return ViewResult();
        }

        public ActionResult UserInfo()
        {
            return ViewResult();
        }

        public ActionResult NotFound()
        {
            return ViewResult();
        }

        public ActionResult Error()
        {
            return ViewResult();
        }
        #endregion

        #region Login
        [HttpPost, AllowAnonymous, Route("signin")]
        public ActionResult SignIn(string userName, string password, bool remember = false)
        {
            var result = Platform.SignIn(userName, password);
            if (!result.IsValid)
                return ErrorResult(result.Message);

            FormsAuthentication.SetAuthCookie(userName, remember);
            return SuccessResult("登录成功！", result.Data);
        }
        #endregion

        #region Index
        [HttpPost, Route("signout")]
        public ActionResult SignOut()
        {
            Session.Clear();
            Platform.SignOut(UserName);
            FormsAuthentication.SignOut();
            return SuccessResult("成功退出！");
        }

        public ActionResult GetMenus()
        {
            var menus = Platform.GetUserMenus(UserName);
            return JsonResult(menus.Select(m => m.ToTree()));
        }
        #endregion

        #region Welcome
        public ActionResult GetTodoLists()
        {
            var data = new CriteriaData();
            return QueryPagingData(data, c => Platform.GetTodoLists(c));
        }

        public ActionResult GetCompanyNews()
        {
            var data = new CriteriaData();
            return QueryPagingData(data, c => Platform.GetCompanyNews(c));
        }

        public ActionResult GetShortCuts()
        {
            return null;
        }
        #endregion

        #region UserInfo
        public ActionResult GetUserInfo()
        {
            return JsonResult(Platform.GetUserInfo(UserName));
        }

        [HttpPost]
        public ActionResult SaveUserInfo(string data)
        {
            return PostAction<dynamic>(data, d => Platform.SaveUserInfo(d));
        }

        [HttpPost]
        public ActionResult UpdatePassword(string oldPassword, string password, string repassword)
        {
            return JsonResult(Platform.UpdatePassword(UserName, oldPassword, password, repassword));
        }
        #endregion

        #region Utils
        public ActionResult Script(int id)
        {
            var script = ResViewEngine.GetScript(id);
            return JavaScript(script);
        }
        #endregion
    }
}

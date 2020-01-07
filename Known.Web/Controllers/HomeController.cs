﻿using System.Web.Mvc;

namespace Known.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Welcome()
        {
            return View();
        }

        public ActionResult Page(string id)
        {
            ViewBag.Id = id;
            return View();
        }

        [AllowAnonymous, Route("login")]
        public ActionResult Login()
        {
            return View();
        }
    }
}
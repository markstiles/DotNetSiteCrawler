using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteIndexer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        //add configuration setup here
        //manage cores
        //site groupings
        //connection settings
        //which url extensions are skipped or allowed

        //then configure a single crawl
        //have a way of using two indexes to prevent and index from going down while crawling
    }
}

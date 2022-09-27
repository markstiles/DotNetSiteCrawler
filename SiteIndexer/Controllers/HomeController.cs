using SiteIndexer.Models.ViewModels;
using SiteIndexer.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteIndexer.Controllers
{
    public class HomeController : Controller
    {
        protected readonly IConfigurationService ConfigurationService;

        public HomeController(IConfigurationService configurationService)
        {
            ConfigurationService = configurationService;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            var model = new HomeViewModel
            {
                Crawlers = ConfigurationService.GetCrawlers()
            };

            return View(model);
        }
    }
}

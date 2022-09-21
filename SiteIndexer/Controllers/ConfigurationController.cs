using SiteIndexer.Factories;
using SiteIndexer.Form.Attributes;
using SiteIndexer.Models.FormModels;
using SiteIndexer.Models.ViewModels;
using SiteIndexer.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteIndexer.Services.Solr;
using SiteIndexer.Services.Models;

namespace SiteIndexer.Controllers
{
    public class ConfigurationController : Controller
    {
        #region Constructor 

        protected readonly IConfigurationService ConfigurationService;
        
        public ConfigurationController(IConfigurationService configurationService)
        {
            ConfigurationService = configurationService;
        }

        #endregion

        #region View Methods

        public ActionResult Index()
        {
            return View();
        }

        #endregion

        #region Post Methods

        [HttpPost]
        [ValidateForm]
        public ActionResult CreateConfiguration(CreateConfigurationFormModel form)
        {
            ConfigurationService.CreateConfiguration(form.ProjectName);

            var result = new CreateConfigurationViewModel
            {
                Succeeded = true
            };

            return Json(result);
        }

        #endregion
    }
}
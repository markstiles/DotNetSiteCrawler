using SiteIndexer.Factories;
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
using SiteIndexer.Services.Configuration;
using SiteIndexer.Models;
using SiteIndexer.Services.Solr.Models;
using SiteIndexer.Models.FormModels.Attributes;
using SiteIndexer.Services.Configuration.Models;
using Azure.Search.Documents.Models;

namespace SiteIndexer.Controllers
{
    public class ConfigurationController : Controller
    {
        #region Constructor 

        protected readonly IConfigurationService ConfigurationService;
        protected readonly ISolrApiService SolrApiService;
        protected readonly IAzureApiService AzureApiService;

        public ConfigurationController(
            IConfigurationService configurationService,
            ISolrApiService solrApiService,
            IAzureApiService azureApiService)
        {
            ConfigurationService = configurationService;
            SolrApiService = solrApiService;
            AzureApiService = azureApiService;
        }

        #endregion

        #region View Methods

        public ActionResult Index()
        {            
            var model = new ConfigurationViewModel
            {
                SolrConnections = ConfigurationService.GetSolrConnections(),
                AzureConnections = ConfigurationService.GetAzureConnections(),
                Sites = ConfigurationService.GetSites()
            };

            return View(model);
        }

        #endregion

        #region Post Methods

        [HttpPost]
        [ValidateForm]
        public ActionResult TestSolrConfiguration(SolrConfigFormModel form)
        {
            var response = SolrApiService.SearchDocuments<DocApiModel>(form.SolrUrl, form.SolrCore, "*:*");
            
            var result = new TransactionResult<DocApiModel[]>
            {
                Succeeded = true,
                ReturnValue = response.response.docs,
                ErrorMessage = string.Empty
            };

            return Json(result);
        }

        [HttpPost]
        [ValidateForm]
        public ActionResult CreateSolrConfiguration(SolrConfigFormModel form)
        {
            var config = ConfigurationService.CreateSolrConnection(Guid.NewGuid(), form.SolrUrl, form.SolrCore);

            var result = new TransactionResult<SolrConnectionModel>
            {
                Succeeded = true,
                ReturnValue = config,
                ErrorMessage = string.Empty
            };

            return Json(result);
        }

        [HttpPost]
        [ValidateForm]
        public ActionResult TestAzureConfiguration(AzureConfigFormModel form)
        {
            var response = AzureApiService.SearchDocuments<DocApiModel>(form.AzureUrl, form.AzureCore, form.AzureApiKey, "*:*");

            var result = new TransactionResult<SearchResults<DocApiModel>>
            {
                Succeeded = true,
                ReturnValue = response.Value,
                ErrorMessage = string.Empty
            };

            return Json(result);
        }

        [HttpPost]
        [ValidateForm]
        public ActionResult CreateAzureConfiguration(AzureConfigFormModel form)
        {
            var config = ConfigurationService.CreateAzureConnection(Guid.NewGuid(), form.AzureUrl, form.AzureCore, form.AzureApiKey);

            var result = new TransactionResult<AzureConnectionModel>
            {
                Succeeded = true,
                ReturnValue = config,
                ErrorMessage = string.Empty
            };

            return Json(result);
        }

        [HttpPost]
        [ValidateForm]
        public ActionResult CreateSiteConfiguration(SiteConfigFormModel form)
        {
            var config = ConfigurationService.CreateSite(Guid.NewGuid(), form.SiteUrl, form.Parser);

            var result = new TransactionResult<SiteModel>
            {
                Succeeded = true,
                ReturnValue = config,
                ErrorMessage = string.Empty
            };

            return Json(result);
        }

        [HttpPost]
        [ValidateForm]
        public ActionResult CreateCrawlingConfiguration(CrawlConfigFormModel form)
        {
            var parts = form.Connection.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
            var type = parts[0]; 
            var connection = Guid.Parse(parts[1]);
            
            var config = ConfigurationService.CreateCrawler(Guid.NewGuid(), form.CrawlerName, connection, form.Sites, type);

            var result = new TransactionResult<CrawlerModel>
            {
                Succeeded = true,
                ReturnValue = config,
                ErrorMessage = string.Empty
            };

            return Json(result);
        }

        #endregion
    }
}
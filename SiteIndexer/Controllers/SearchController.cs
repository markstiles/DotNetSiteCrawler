using SiteIndexer.Models;
using SiteIndexer.Models.ViewModels;
using SiteIndexer.Services.Configuration;
using SiteIndexer.Services.Solr;
using SiteIndexer.Services.Solr.Models;
using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using SiteIndexer.Services.Azure.Models;

namespace SiteIndexer.Controllers
{
    public class SearchController : Controller
    {
        #region Constructor 

        protected readonly IConfigurationService ConfigurationService;
        protected readonly ISolrApiService SolrApiService;
        protected readonly IAzureApiService AzureApiService;

        public SearchController(
            IConfigurationService configurationService, 
            ISolrApiService solrApiService,
            IAzureApiService azureApiService)
        {
            ConfigurationService = configurationService;
            SolrApiService = solrApiService;
            AzureApiService = azureApiService;
        }

        #endregion

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            var model = new SearchViewModel
            {
                SolrConnections = ConfigurationService.GetSolrConnections(),
                AzureConnections = ConfigurationService.GetAzureConnections()
            };

            return View(model);
        }

        #region Post Methods

        [HttpPost]
        public ActionResult Search(Guid connectionId, string query)
        {
            var solrConfig = ConfigurationService.GetSolrConnection(connectionId);
            var azureConfig = ConfigurationService.GetAzureConnection(connectionId);
            if (solrConfig != null)
            {
                var searchQuery = string.IsNullOrWhiteSpace(query) ? "*:*" : $"title:{query} or content:{query}";
                var response = SolrApiService.SearchDocuments<DocApiModel>(solrConfig.Url, solrConfig.Core, searchQuery);

                var result = new TransactionResult<DocApiModel[]>
                {
                    Succeeded = true,
                    ReturnValue = response?.response?.docs ?? new DocApiModel[0],
                    ErrorMessage = string.Empty
                };

                return Json(result);
            }
            else if(azureConfig != null)
            {
                var response = AzureApiService.SearchDocuments<AzureDocumentApiModel>(azureConfig.Url, azureConfig.Core, azureConfig.ApiKey, "");

                var searchResults = response.Value.GetResults().Select(a => a.Document).ToArray();

                var result = new TransactionResult<AzureDocumentApiModel[]>
                {                    
                    Succeeded = true,
                    ReturnValue = searchResults,
                    ErrorMessage = string.Empty
                };

                return Json(result);
            }

            return Json(new TransactionResult<DocApiModel[]>
            {
                Succeeded = true,
                ErrorMessage = "There was no valid config found"
            });
        }

        #endregion
    }
}
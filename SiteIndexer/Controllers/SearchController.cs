using SiteIndexer.Models;
using SiteIndexer.Models.ViewModels;
using SiteIndexer.Services.Configuration;
using SiteIndexer.Services.Solr;
using SiteIndexer.Services.Solr.Models;
using System;
using System.Web.Mvc;

namespace SiteIndexer.Controllers
{
    public class SearchController : Controller
    {
        #region Constructor 

        protected readonly IConfigurationService ConfigurationService;
        protected readonly ISolrApiService SolrApiService;

        public SearchController(
            IConfigurationService configurationService, 
            ISolrApiService solrApiService)
        {
            ConfigurationService = configurationService;
            SolrApiService = solrApiService;
        }

        #endregion

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            var model = new SearchViewModel
            {
                SolrConnections = ConfigurationService.GetSolrConnections()
            };

            return View(model);
        }

        #region Post Methods

        [HttpPost]
        public ActionResult Search(Guid solrConnectionId, string query)
        {
            var config = ConfigurationService.GetSolrConnection(solrConnectionId);
            var searchQuery = string.IsNullOrWhiteSpace(query) ? "*:*" : $"title:{query} or content:{query}";
            var response = SolrApiService.SearchDocuments<DocApiModel>(config.Url, config.Core, searchQuery);

            var result = new TransactionResult<DocApiModel[]>
            {
                Succeeded = true,
                ReturnValue = response?.response?.docs ?? new DocApiModel[0],
                ErrorMessage = string.Empty
            };

            return Json(result);
        }

        #endregion
    }
}
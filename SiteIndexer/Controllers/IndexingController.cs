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
using SiteIndexer.Services.Configuration.Models;
using SiteIndexer.Services.Configuration;
using SiteIndexer.Services.Crawling;
using SiteIndexer.Models;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text;
using System.Net.Http.Headers;
using SiteIndexer.Services.Solr.Models;
using SiteIndexer.Services.System;
using SiteIndexer.Services.Indexing;

namespace SiteIndexer.Controllers
{
    public class IndexingController : Controller
    {
        #region Constructor 

        protected readonly IConfigurationService ConfigurationService;
        protected readonly ISolrApiService SolrApiService;
        protected readonly ICrawlingService CrawlingService;
        protected readonly IStringService StringService;
        protected readonly IIndexingService IndexingService;

        public IndexingController(
            IConfigurationService configurationService, 
            ISolrApiService solrApiService,
            ICrawlingService crawlingService,
            IStringService stringService,
            IIndexingService indexingService)
        {
            ConfigurationService = configurationService;
            SolrApiService = solrApiService;
            CrawlingService = crawlingService;
            StringService = stringService;
            IndexingService = indexingService;
        }

        #endregion

        #region Post Methods

        [HttpPost]
        [ValidateForm]
        public ActionResult Start()
        {
            //TODO make the start domain come from configuration list
            var domainList = new List<string>
            {
                "https://markstiles.net",
                "http://projectinsights.local"
            };
            var updatedDate = DateTime.Now.ToString("yyy-MM-ddThh:mm:ssZ");

            var isIndexed = new Dictionary<string, Uri>();
            foreach (var domain in domainList)
            {
                var startUri = new Uri($"{domain}/");
                var toIndex = new Dictionary<string, Uri>
                {
                    { StringService.GetValidKey(startUri), startUri }
                };
                
                while (toIndex.Count > 0)
                {
                    var firstEntry = toIndex.First();
                    var currentUri = firstEntry.Value;

                    //update this page as crawled
                    isIndexed.Add(firstEntry.Key, currentUri);
                    toIndex.Remove(firstEntry.Key);

                    //query page for content
                    var html = CrawlingService.GetHtml(currentUri);
             
                    //gather all the links and determine what has been crawled or not
                    var validLinks = CrawlingService.GetValidLinks(html, currentUri);
                    foreach(var uri in validLinks)
                    {
                        var validKey = StringService.GetValidKey(uri);
                        if (toIndex.ContainsKey(validKey) || isIndexed.ContainsKey(validKey))
                            continue;
                        
                        toIndex.Add(validKey, uri);
                    }
                    
                    //TODO batch update items so there aren't so many calls
                    //index item
                    IndexingService.IndexItem(html, currentUri, updatedDate);
                }
            }

            //remove anything from solr that wasn't updated
            SolrApiService.DeleteDocumentsByQuery($"-updated:{updatedDate}");

            var result = new TransactionResult<object>
            {
                Succeeded = true,
                ReturnValue = new { links = isIndexed },
                ErrorMessage = string.Empty
            };

            return Json(result);
        }

        #endregion
    }
}
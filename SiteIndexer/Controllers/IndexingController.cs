using SiteIndexer.Factories;
using SiteIndexer.Models.FormModels.Attributes;
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
using System.Threading;
using SiteIndexer.Services.Jobs.Models;
using SiteIndexer.Services.Jobs;

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
        protected readonly IJobService JobService;

        public IndexingController(
            IConfigurationService configurationService, 
            ISolrApiService solrApiService,
            ICrawlingService crawlingService,
            IStringService stringService,
            IIndexingService indexingService,
            IJobService jobService)
        {
            ConfigurationService = configurationService;
            SolrApiService = solrApiService;
            CrawlingService = crawlingService;
            StringService = stringService;
            IndexingService = indexingService;
            JobService = jobService;
        }

        #endregion

        #region Post Methods

        [HttpPost]
        [ValidateForm]
        public ActionResult Start(Guid CrawlerId)
        {
            var handleName = JobService.StartJob(CrawlerId, ProcessConfiguration);
            
            var result = new TransactionResult<object>
            {
                Succeeded = true,
                ReturnValue = handleName,
                ErrorMessage = string.Empty
            };

            return Json(result);
        }

        [HttpPost]
        public ActionResult GetJobStatus(string handleName, DateTime lastDateReceived)
        {
            var status = JobService.GetJobStatus(handleName, lastDateReceived);

            return Json(new { JobStatus = status });
        }

        [HttpPost]
        public ActionResult EmptyIndex(Guid solrConnectionId)
        {
            var config = ConfigurationService.GetSolrConnection(solrConnectionId);
            var response = SolrApiService.DeleteAllDocuments(config.Url, config.Core);

            var result = new TransactionResult<SolrUpdateResponseApiModel>
            {
                Succeeded = true,
                ReturnValue = response,
                ErrorMessage = string.Empty
            };

            return Json(result);
        }

        #endregion

        public void ProcessConfiguration(Guid crawlerId, MessageList messages)
        {
            var config = ConfigurationService.GetCrawler(crawlerId);
            var domainList = config.Sites.Select(a => ConfigurationService.GetSite(a).Url).ToList();
            var solrConfig = ConfigurationService.GetSolrConnection(config.SolrConnection);
            var updatedDate = DateTime.Now.ToString("yyy-MM-ddThh:mm:ssZ");

            var isIndexed = new Dictionary<string, Uri>();
            foreach (var domain in domainList)
            {
                messages.Add($"Starting to crawl: {domain}");
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
                    foreach (var uri in validLinks)
                    {
                        var validKey = StringService.GetValidKey(uri);
                        if (toIndex.ContainsKey(validKey) || isIndexed.ContainsKey(validKey))
                            continue;

                        toIndex.Add(validKey, uri);
                    }

                    //TODO batch update items so there aren't so many calls
                    //index item
                    IndexingService.IndexItem(solrConfig.Url, solrConfig.Core, html, currentUri, updatedDate);

                    messages.Add($"Found: {(toIndex.Count + isIndexed.Count)} - Crawled: {isIndexed.Count} - Remaining - {toIndex.Count}");
                }
            }

            //remove anything from solr that wasn't updated
            SolrApiService.DeleteDocumentsByQuery(solrConfig.Url, solrConfig.Core, $"-updated:{updatedDate}");
        }
    }
}
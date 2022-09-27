using SiteIndexer.Factories;
using SiteIndexer.Models;
using SiteIndexer.Models.FormModels.Attributes;
using SiteIndexer.Services.Configuration;
using SiteIndexer.Services.Crawling;
using SiteIndexer.Services.Indexing;
using SiteIndexer.Services.Jobs;
using SiteIndexer.Services.Jobs.Models;
using SiteIndexer.Services.Solr;
using SiteIndexer.Services.Solr.Models;
using SiteIndexer.Services.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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
        protected readonly ISiteCrawlerFactory SiteCrawlerFactory;

        public IndexingController(
            IConfigurationService configurationService, 
            ISolrApiService solrApiService,
            ICrawlingService crawlingService,
            IStringService stringService,
            IIndexingService indexingService,
            IJobService jobService,
            ISiteCrawlerFactory siteCrawlerFactory)
        {
            ConfigurationService = configurationService;
            SolrApiService = solrApiService;
            CrawlingService = crawlingService;
            StringService = stringService;
            IndexingService = indexingService;
            JobService = jobService;
            SiteCrawlerFactory = siteCrawlerFactory;
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
        public ActionResult EmptyIndex(Guid CrawlerId)
        {
            var crawler = ConfigurationService.GetCrawler(CrawlerId);
            var solr = ConfigurationService.GetSolrConnection(crawler.SolrConnection);
            var response = SolrApiService.DeleteAllDocuments(solr.Url, solr.Core);

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
            var siteList = config.Sites.Select(a => ConfigurationService.GetSite(a));
            var solrConfig = ConfigurationService.GetSolrConnection(config.SolrConnection);
            var updatedDate = DateTime.Now.ToString("yyy-MM-ddThh:mm:ssZ");

            var isIndexed = new Dictionary<string, Uri>();
            foreach (var site in siteList)
            {
                var parser = SiteCrawlerFactory.Create(site.Parser);
                
                messages.Add($"Starting to crawl: {site.Url}");
                var startUri = new Uri($"{site.Url}/");
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
                    var validLinks = CrawlingService.GetValidLinks(html, currentUri, parser.GetAllowedPageExtensions());
                    foreach (var uri in validLinks)
                    {
                        var validKey = StringService.GetValidKey(uri);
                        if (toIndex.ContainsKey(validKey) || isIndexed.ContainsKey(validKey))
                            continue;

                        toIndex.Add(validKey, uri);
                    }

                    //TODO batch update items so there aren't so many calls
                    //index item
                    IndexingService.IndexItem(parser, solrConfig.Url, solrConfig.Core, html, currentUri, updatedDate);

                    messages.Add($"Found: {(toIndex.Count + isIndexed.Count)} - Crawled: {isIndexed.Count} - Remaining - {toIndex.Count}");
                }
            }

            //remove anything from solr that wasn't updated
            SolrApiService.DeleteDocumentsByQuery(solrConfig.Url, solrConfig.Core, $"-updated:{updatedDate}");
        }
    }
}
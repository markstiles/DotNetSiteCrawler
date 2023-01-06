using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using SiteIndexer.Factories;
using SiteIndexer.Models;
using SiteIndexer.Models.FormModels.Attributes;
using SiteIndexer.Services.Azure.Models;
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
        protected readonly IAzureApiService AzureApiService;
        protected readonly ICrawlingService CrawlingService;
        protected readonly IStringService StringService;
        protected readonly IJobService JobService;
        protected readonly ISiteParserFactory SiteCrawlerFactory;

        public IndexingController(
            IConfigurationService configurationService, 
            ISolrApiService solrApiService,
            IAzureApiService azureApiService,
            ICrawlingService crawlingService,
            IStringService stringService,
            IJobService jobService,
            ISiteParserFactory siteCrawlerFactory)
        {
            ConfigurationService = configurationService;
            SolrApiService = solrApiService;
            AzureApiService = azureApiService;
            CrawlingService = crawlingService;
            StringService = stringService;
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
            
            if(crawler.Type == "solr")
            {
                var solr = ConfigurationService.GetSolrConnection(crawler.Connection);
                var response = SolrApiService.DeleteAllDocuments(solr.Url, solr.Core);

                var result = new TransactionResult<SolrUpdateResponseApiModel>
                {
                    Succeeded = true,
                    ReturnValue = response,
                    ErrorMessage = string.Empty
                };

                return Json(result);
            }
            else if (crawler.Type == "azure")
            {
                var azure = ConfigurationService.GetAzureConnection(crawler.Connection);
                var response = AzureApiService.DeleteAllDocuments(azure.Url, azure.Core, azure.ApiKey);

                var result = new TransactionResult<Response<IndexDocumentsResult>>
                {
                    Succeeded = true,
                    ReturnValue = response,
                    ErrorMessage = string.Empty
                };

                return Json(result);
            }

            return Json(null);
        }

        #endregion

        public void ProcessConfiguration(Guid crawlerId, MessageList messages)
        {
            var crawler = ConfigurationService.GetCrawler(crawlerId);
            var siteList = crawler.Sites.Select(a => ConfigurationService.GetSite(a)).ToList();

            var now = DateTime.Now;
            var updatedDate = now.ToString("yyyy-MM-ddTHH:mm:ssZ");

            messages.Add($"Found {siteList.Count} sites to crawl and index");

            var isIndexed = new Dictionary<string, Uri>();
            foreach (var site in siteList)
            {
                var parser = SiteCrawlerFactory.Create(site.Parser);
                
                messages.Add($"Starting: {site.Url}");
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
                    var title = parser.GetTitle(html);
                    var content = parser.GetContent(html);

                    if (crawler.Type == "solr")
                    {
                        var model = new SolrDocumentApiModel
                        {
                            id = StringService.GetValidKey(currentUri),
                            title = title,
                            content = content,
                            url = currentUri.AbsoluteUri,
                            updated = updatedDate
                        };
                        var solrConfig = ConfigurationService.GetSolrConnection(crawler.Connection);
                        SolrApiService.AddDocuments(solrConfig.Url, solrConfig.Core, new List<SolrDocumentApiModel> { model });
                    }
                    else if (crawler.Type == "azure")
                    {
                        var model = new AzureDocumentApiModel
                        {
                            id = StringService.GetValidKey(currentUri),
                            Title = title,
                            Content = content,
                            Url = currentUri.AbsoluteUri,
                            Updated = now
                        };
                        var azureConfig = ConfigurationService.GetAzureConnection(crawler.Connection);
                        AzureApiService.AddDocuments(azureConfig.Url, azureConfig.Core, azureConfig.ApiKey, new List<AzureDocumentApiModel> { model });
                    }

                    messages.Add($"Found: {(toIndex.Count + isIndexed.Count)} - Crawled: {isIndexed.Count} - Remaining - {toIndex.Count}");
                }

                messages.Add($"Finished: {site.Url}");
            }

            //remove anything from solr that wasn't updated
            if (crawler.Type == "solr")
            {
                var solrConfig = ConfigurationService.GetSolrConnection(crawler.Connection);
                SolrApiService.DeleteDocumentsByQuery(solrConfig.Url, solrConfig.Core, $"-updated:{updatedDate}");
            }
            else if (crawler.Type == "azure")
            {
                var azureConfig = ConfigurationService.GetAzureConnection(crawler.Connection);
                AzureApiService.DeleteDocumentsByQuery(azureConfig.Url, azureConfig.Core, azureConfig.ApiKey, $"Updated lt {updatedDate}");
            }
        }
    }
}
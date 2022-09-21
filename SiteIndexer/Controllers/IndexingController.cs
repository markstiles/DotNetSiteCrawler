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
using SiteIndexer.Models;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text;
using System.Net.Http.Headers;

namespace SiteIndexer.Controllers
{
    public class IndexingController : Controller
    {
        #region Constructor 

        protected readonly IConfigurationService ConfigurationService;
        protected readonly IHttpClientFactory ClientFactory;


        public IndexingController(IConfigurationService configurationService, IHttpClientFactory clientFactory)
        {
            ConfigurationService = configurationService;
            ClientFactory = clientFactory;
        }

        #endregion

        #region Post Methods

        [HttpPost]
        [ValidateForm]
        public ActionResult Start()
        {
            //TODO move solr settings to configuration
            var solrClient = ClientFactory.CreateClient();
            solrClient.BaseAddress = new Uri("http://localhost:900");

            var updatedDate = DateTime.Now.ToString("yyy-MM-ddThh:mm:ssZ");

            //setup domain
            //TODO make the start domain come from configuration list
            var startDomain = "https://markstiles.net";
            var startUri = new Uri($"{startDomain}/");
            var toIndex = new Dictionary<string, Uri>
            {
                { GetValidKey(startUri), startUri }
            };
            var isIndexed = new Dictionary<string, Uri>();

            while (toIndex.Count > 0)
            {
                var firstEntry = toIndex.First();
                var currentUri = firstEntry.Value;

                //update this page as crawled
                isIndexed.Add(firstEntry.Key, currentUri);
                toIndex.Remove(firstEntry.Key);

                //query page for content
                var html = GetHtml(currentUri);
             
                //gather all the links and determine what has been crawled or not
                var validLinks = GetValidLinks(html, currentUri);
                foreach(var uri in validLinks)
                {
                    var validKey = GetValidKey(uri);
                    if (toIndex.ContainsKey(validKey) || isIndexed.ContainsKey(validKey))
                        continue;
                        
                    toIndex.Add(validKey, uri);
                }

                //index item
                IndexItem(solrClient, html, currentUri, updatedDate);
            }

            //TODO remove anything from solr that wasn't updated

            var result = new TransactionResult<object>
            {
                Succeeded = true,
                ReturnValue = new { links = isIndexed },
                ErrorMessage = string.Empty
            };

            return Json(result);
        }

        #endregion

        protected string GetValidKey(Uri uri)
        {
            return uri.AbsoluteUri.Replace($"{uri.Scheme}://", "").Replace("www.", "");
        }

        protected List<Uri> GetValidLinks(HtmlDocument html, Uri htmlUri)
        {
            var links = new List<Uri>();
            
            var anchors = html.DocumentNode.SelectNodes("//a");
            if (anchors == null || anchors.Count == 0)
                return links;

            foreach (var link in anchors)
            {
                var href = link.GetAttributeValue("href", "");
                var isAbsoluteUri = href.StartsWith("htt");
                var isExternal = isAbsoluteUri && !href.StartsWith($"{htmlUri.Scheme}://{htmlUri.Host}");
                if (string.IsNullOrWhiteSpace(href) || href.StartsWith("#") || href.StartsWith("?") || isExternal || href.Contains("@"))
                    continue;

                href = href.Replace("://", ":##").Replace("//", "/").Replace(":##", "://");

                if (href.Contains("#"))
                    href = href.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries).First();

                var separator = href.StartsWith("/") ? "" : "/";
                var linkUri = new Uri(isAbsoluteUri ? href : $"{htmlUri.Scheme}://{htmlUri.Host}{separator}{href}");
                
                //TODO add allowed extensions to configuration
                var allowedExtensions = new List<string>
                {
                    "php", "htm", "html", "jsp", "asp", "aspx"
                };

                var fileExtension = GetExtension(linkUri);
                var hasExtension = !string.IsNullOrWhiteSpace(fileExtension);
                var notAllowed = !allowedExtensions.Contains(fileExtension);
                if (hasExtension && notAllowed)
                    continue;

                links.Add(linkUri);
            }

            return links;
        }

        protected string GetExtension(Uri uri)
        {
            var lastSegment = uri.Segments.LastOrDefault();
            if (lastSegment == null)
                return string.Empty;

            var period = ".";
            if (!lastSegment.Contains(period))
                return string.Empty;

            var segmentParts = lastSegment.Split(new string[] { period }, StringSplitOptions.RemoveEmptyEntries);
            if (segmentParts.Length == 0)
                return string.Empty;

            return segmentParts.Last();
        }

        protected void IndexItem(HttpClient client, HtmlDocument html, Uri currentUri, string updatedDate)
        {
            var body = html.DocumentNode.SelectSingleNode("//body");
            var bodyText = "";
            if(body != null)
            {
                var nodesToRemove = body.SelectNodes("//a").ToList();
                foreach (var node in nodesToRemove)
                    node.Remove();

                bodyText = string.Join(" ", body.InnerText.Split(new string[] { " ", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries));

                //TODO strip stop words and punctuation
            }
            
            var metatags = html.DocumentNode.SelectNodes("//meta");
            //TODO build title with fallbacks to og title or meta title or h1
            var title = html.DocumentNode.SelectSingleNode("/html/head/title");

            //TODO build with factory
            var model = new SolrSchemaApiModel
            {
                id = GetValidKey(currentUri),
                title = title?.InnerText ?? "",
                content = bodyText,
                url = currentUri.AbsoluteUri,
                updated = updatedDate
            };

            AddToSolr(client, new List<SolrSchemaApiModel> { model });
        }

        protected HtmlDocument GetHtml(Uri uri)
        {
            var client = ClientFactory.CreateClient();
            client.DefaultRequestHeaders.Clear();
            using (var res = Task.Run(() => client.GetAsync(uri)))
            {
                res.Wait();
                var content = res.Result.Content.ReadAsStringAsync().Result;

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(content);

                return htmlDoc;
            }
        }

        protected SolrUpdateResponseApiModel AddToSolr(HttpClient client, List<SolrSchemaApiModel> models)
        {
            var apiUrl = "/solr/webcrawl/update?commitWithin=1000";
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var serialContent = JsonConvert.SerializeObject(models, settings);
            var JsonContentType = "application/json";
            var paramContent = new StringContent(serialContent, Encoding.UTF8, JsonContentType);
            client.DefaultRequestHeaders.Clear();
            //client.DefaultRequestHeaders.Add("Content-Type", JsonContentType);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonContentType));

            using (var res = Task.Run(() => client.PostAsync(apiUrl, paramContent)))
            {
                res.Wait();
                var content = res.Result.Content.ReadAsStringAsync().Result;
                var response = JsonConvert.DeserializeObject<SolrUpdateResponseApiModel>(content);

                return response;
            }
        }
    }

    public class SolrUpdateResponseApiModel
    {
        public ResponseHeaderApiModel responseHeader { get; set; }
    }

    public class ResponseHeaderApiModel
    {
        public int status { get; set; }
        public int QTime { get; set; }
    }

    public class SolrSchemaApiModel
    {
        public string id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string url { get; set; }
        public string updated { get; set; }
    }
}
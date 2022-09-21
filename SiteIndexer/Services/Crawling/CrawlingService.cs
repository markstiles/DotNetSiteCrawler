using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SiteIndexer.Services.Crawling
{
    public interface ICrawlingService
    {
        List<Uri> GetValidLinks(HtmlDocument html, Uri htmlUri);
        string GetExtension(Uri uri);
        HtmlDocument GetHtml(Uri uri);
    }

    public class CrawlingService : ICrawlingService
    {
        protected readonly HttpClient Client;

        public CrawlingService(IHttpClientFactory clientFactory)
        {
            Client = clientFactory.CreateClient();
            //Client.BaseAddress = new Uri("http://localhost:900");
        }

        public List<Uri> GetValidLinks(HtmlDocument html, Uri htmlUri)
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

        public string GetExtension(Uri uri)
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

        public HtmlDocument GetHtml(Uri uri)
        {
            Client.DefaultRequestHeaders.Clear();
            using (var res = Task.Run(() => Client.GetAsync(uri)))
            {
                res.Wait();
                var content = res.Result.Content.ReadAsStringAsync().Result;

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(content);

                return htmlDoc;
            }
        }
    }
}
using HtmlAgilityPack;
using SiteIndexer.Services.Solr;
using SiteIndexer.Services.Solr.Models;
using SiteIndexer.Services.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SiteIndexer.Services.Indexing
{
    public interface IIndexingService
    {
        SolrUpdateResponseApiModel IndexItem(string solrUrl, string solrCore, HtmlDocument html, Uri currentUri, string updatedDate);
    }

    public class IndexingService : IIndexingService
    {
        protected readonly ISolrApiService SolrApiService;
        protected readonly IStringService StringService;

        public IndexingService(
            ISolrApiService solrApiService,
            IStringService stringService)
        {
            SolrApiService = solrApiService;
            StringService = stringService;
        }

        public SolrUpdateResponseApiModel IndexItem(string solrUrl, string solrCore, HtmlDocument html, Uri currentUri, string updatedDate)
        {
            var body = html.DocumentNode.SelectSingleNode("//body");
            var bodyText = "";
            if (body != null)
            {
                var nodesToRemove = body.SelectNodes("//a").ToList();
                foreach (var node in nodesToRemove)
                    node.Remove();

                var bodyInnerText = string.Join(" ", body.InnerText.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries));

                var charArr = bodyInnerText.ToCharArray();
                StringBuilder sb = new StringBuilder();
                foreach (char c in charArr)
                {
                    //TODO need to break this into two parts, one field for searching and one field for result display
                    if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
                        continue;

                    sb.Append(c);
                }

                bodyText = string.Join(" ", sb.ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));

                //TODO add site crawler profile to handle getting content differently for each site
            }

            var metatags = html.DocumentNode.SelectNodes("//meta");
            //TODO build title with fallbacks to og title or meta title or h1
            var title = html.DocumentNode.SelectSingleNode("/html/head/title");

            //TODO build with factory
            var model = new SolrDocumentApiModel
            {
                id = StringService.GetValidKey(currentUri),
                title = title?.InnerText ?? "",
                content = bodyText,
                url = currentUri.AbsoluteUri,
                updated = updatedDate
            };

            return SolrApiService.AddDocuments(solrUrl, solrCore, new List<SolrDocumentApiModel> { model });
        }
    }
}
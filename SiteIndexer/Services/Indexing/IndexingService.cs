using HtmlAgilityPack;
using SiteIndexer.Services.Indexing.SiteParsers;
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
        SolrUpdateResponseApiModel IndexItem(ISiteParser parser, string solrUrl, string solrCore, HtmlDocument html, Uri currentUri, string updatedDate);
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

        public SolrUpdateResponseApiModel IndexItem(ISiteParser parser, string solrUrl, string solrCore, HtmlDocument html, Uri currentUri, string updatedDate)
        {
            var title = parser.GetTitle(html);
            var content = parser.GetContent(html);
            
            var model = new SolrDocumentApiModel
            {
                id = StringService.GetValidKey(currentUri),
                title = title,
                content = content,
                url = currentUri.AbsoluteUri,
                updated = updatedDate
            };

            return SolrApiService.AddDocuments(solrUrl, solrCore, new List<SolrDocumentApiModel> { model });
        }
    }
}
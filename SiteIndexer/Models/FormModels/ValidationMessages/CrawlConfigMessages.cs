using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Models.FormModels.ValidationMessages
{
    public static class CrawlConfigMessages
    {
        public const string CrawlNameRequired = "You must provide a Solr URL";
        public const string SolrConnectionRequired = "You must provide a Solr Core";
    }
}
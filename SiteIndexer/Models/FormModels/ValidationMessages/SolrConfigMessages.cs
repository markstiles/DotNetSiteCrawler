using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Models.FormModels.ValidationMessages
{
    public static class SolrConfigMessages
    {
        public const string SolrUrlRequired = "You must provide a Solr URL";
        public const string SolrCoreRequired = "You must provide a Solr Core";
    }
}
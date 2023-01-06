using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Models.FormModels.ValidationMessages
{
    public static class ConfigMessages
    {
        public const string SolrUrlRequired = "You must provide a Solr URL";
        public const string SolrCoreRequired = "You must provide a Solr Core";
        public const string AzureUrlRequired = "You must provide an Azure URL";
        public const string AzureCoreRequired = "You must provide an Azure Core";
        public const string AzureApiKeyRequired = "You must provide an Azure API Key";
    }
}
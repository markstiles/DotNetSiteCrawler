using SiteIndexer.Services.Configuration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Models.ViewModels
{
    public class ConfigurationViewModel
    {
        public List<SolrConnectionModel> SolrConnections { get; set; }
        public List<AzureConnectionModel> AzureConnections { get; set; }
        public List<SiteModel> Sites { get; set; }
    }
}
using SiteIndexer.Services.Configuration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<CrawlerModel> Crawlers { get; set; }
        public List<SolrConnectionModel> SolrConnections { get; set; } 
    }
}
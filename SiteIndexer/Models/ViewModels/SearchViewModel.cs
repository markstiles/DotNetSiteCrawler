using SiteIndexer.Services.Configuration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Models.ViewModels
{
    public class SearchViewModel
    {
        public List<SolrConnectionModel> SolrConnections { get; set; } 
    }
}
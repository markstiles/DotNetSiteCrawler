using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Services.Configuration.Models
{
    public class SolrConnectionModel
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Core { get; set; }
    }
}
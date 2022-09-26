using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Services.Configuration.Models
{
    public class CrawlerModel
    {
        public Guid Id { get; set; }
        public string CrawlerName { get; set; }
        public Guid SolrConnection { get; set; }
        public List<Guid> Sites { get; set; }
    }
}
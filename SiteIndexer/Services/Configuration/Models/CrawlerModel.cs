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
        public Guid Connection { get; set; }
        public List<Guid> Sites { get; set; }
        public string Type { get; set; }
    }
}
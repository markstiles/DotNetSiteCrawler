using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Services.Solr.Models
{
    public class DocApiModel
    {
        public string id { get; set; }
        public string[] title { get; set; }
        public string[] content { get; set; }
        public string[] url { get; set; }
        public DateTime[] updated { get; set; }
        public long _version_ { get; set; }
    }
}
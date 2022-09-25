using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Services.Solr.Models
{
    public class SolrQueryResponseApiModel<T>
    {
        public ResponseHeaderApiModel responseHeader { get; set; }
        public ResponseApiModel<T> response { get; set; }
    }
}
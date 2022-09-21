using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Services.Solr.Models
{
    public class QueryApiModel
    {
        public QueryApiModel(string solrQuery)
        {
            query = solrQuery;
        }

        public QueryApiModel()
        {

        }

        public string query { get; set; }
    }
}
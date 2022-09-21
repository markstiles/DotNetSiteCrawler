using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Services.Solr.Models
{
    public class DeleteQueryApiModel
    {
        public DeleteQueryApiModel(string solrQuery)
        {
            delete = new QueryApiModel(solrQuery);
        }

        public DeleteQueryApiModel()
        {
            delete = new QueryApiModel();
        }

        public QueryApiModel delete { get; set; }
    }
}
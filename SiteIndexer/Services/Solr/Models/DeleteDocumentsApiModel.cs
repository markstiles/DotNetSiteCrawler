using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Services.Solr.Models
{
    public class DeleteDocumentsApiModel
    {
        public DeleteDocumentsApiModel(List<SolrDocumentApiModel> models)
        {
            delete = models;
        }

        public DeleteDocumentsApiModel()
        {
            delete = new List<SolrDocumentApiModel>();
        }

        public List<SolrDocumentApiModel> delete { get; set; }
    }
}
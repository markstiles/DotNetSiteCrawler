using SiteIndexer.Services.Solr.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace SiteIndexer.Services.Solr
{
    public interface ISolrApiService
    {
        SolrUpdateResponseApiModel AddDocuments(List<SolrDocumentApiModel> models);
        SolrUpdateResponseApiModel DeleteDocuments(List<SolrDocumentApiModel> models);
        SolrUpdateResponseApiModel DeleteDocumentsByQuery(string solrQuery);
        SolrUpdateResponseApiModel DeleteAllDocuments();
        SolrQueryResponseApiModel<T> SearchDocuments<T>(string query, int rows = 10);
    }

    public class SolrApiService : ISolrApiService
    {
        #region Constructor

        protected readonly ISolrClient Client;
        protected readonly string SolrCore;

        public SolrApiService(ISolrClient client)
        {
            Client = client;
            //TODO move this to config
            SolrCore = "webcrawl";
        }

        #endregion

        #region Context

        public SolrUpdateResponseApiModel AddDocuments(List<SolrDocumentApiModel> models)
        {
            var apiUrl = $"/solr/{SolrCore}/update?commitWithin=1000";
            var response = Client.SendPost<SolrUpdateResponseApiModel>(apiUrl, models);

            return response;
        }

        public SolrUpdateResponseApiModel DeleteDocuments(List<SolrDocumentApiModel> models)
        {
            var apiUrl = $"/solr/{SolrCore}/update?commit=true";
            var deleteModel = new DeleteDocumentsApiModel(models);
            var response = Client.SendPost<SolrUpdateResponseApiModel>(apiUrl, deleteModel);

            return response;
        }

        public SolrUpdateResponseApiModel DeleteDocumentsByQuery(string solrQuery)
        {
            var apiUrl = $"/solr/{SolrCore}/update?commit=true";
            var deleteModel = new DeleteQueryApiModel(solrQuery);
            var response = Client.SendPost<SolrUpdateResponseApiModel>(apiUrl, deleteModel);

            return response;
        }

        public SolrUpdateResponseApiModel DeleteAllDocuments()
        {
            var apiUrl = $"/solr/{SolrCore}/update?commit=true";
            var deleteModel = new DeleteQueryApiModel("*:*");
            var response = Client.SendPost<SolrUpdateResponseApiModel>(apiUrl, deleteModel);

            return response;
        }

        public SolrQueryResponseApiModel<T> SearchDocuments<T>(string query, int rows = 10)
        {
            var apiUrl = $"/solr/{SolrCore}/select?q={query}&rows={rows}";
            var response = Client.SendGet<SolrQueryResponseApiModel<T>>(apiUrl);

            return response;
        }

        #endregion
    }
}
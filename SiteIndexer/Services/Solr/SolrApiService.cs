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
        SolrUpdateResponseApiModel AddDocuments(string url, string core, List<SolrDocumentApiModel> models);
        SolrUpdateResponseApiModel DeleteDocuments(string url, string core, List<SolrDocumentApiModel> models);
        SolrUpdateResponseApiModel DeleteDocumentsByQuery(string url, string core, string solrQuery);
        SolrUpdateResponseApiModel DeleteAllDocuments(string url, string core);
        SolrQueryResponseApiModel<T> SearchDocuments<T>(string url, string core, string query, int rows = 10);
    }

    public class SolrApiService : ISolrApiService
    {
        #region Constructor

        protected readonly ISolrClient Client;
        protected readonly string SolrCore;

        public SolrApiService(ISolrClient client)
        {
            Client = client;
        }

        #endregion

        #region Context

        public SolrUpdateResponseApiModel AddDocuments(string url, string core, List<SolrDocumentApiModel> models)
        {
            var apiUrl = $"{url}/solr/{core}/update?commitWithin=1000";
            var response = Client.SendPost<SolrUpdateResponseApiModel>(apiUrl, models);

            return response;
        }

        public SolrUpdateResponseApiModel DeleteDocuments(string url, string core, List<SolrDocumentApiModel> models)
        {
            var apiUrl = $"{url}/solr/{core}/update?commit=true";
            var deleteModel = new DeleteDocumentsApiModel(models);
            var response = Client.SendPost<SolrUpdateResponseApiModel>(apiUrl, deleteModel);

            return response;
        }

        public SolrUpdateResponseApiModel DeleteDocumentsByQuery(string url, string core, string solrQuery)
        {
            var apiUrl = $"{url}/solr/{core}/update?commit=true";
            var deleteModel = new DeleteQueryApiModel(solrQuery);
            var response = Client.SendPost<SolrUpdateResponseApiModel>(apiUrl, deleteModel);

            return response;
        }

        public SolrUpdateResponseApiModel DeleteAllDocuments(string url, string core)
        {
            var apiUrl = $"{url}/solr/{core}/update?commit=true";
            var deleteModel = new DeleteQueryApiModel("*:*");
            var response = Client.SendPost<SolrUpdateResponseApiModel>(apiUrl, deleteModel);

            return response;
        }

        public SolrQueryResponseApiModel<T> SearchDocuments<T>(string url, string core, string query, int rows = 10)
        {
            var apiUrl = $"{url}/solr/{core}/select?q={query}&rows={rows}";
            var response = Client.SendGet<SolrQueryResponseApiModel<T>>(apiUrl);

            return response;
        }

        #endregion
    }
}
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using SiteIndexer.Services.Azure.Models;

namespace SiteIndexer.Services.Solr
{
    public interface IAzureApiService
    {
        Response<IndexDocumentsResult> AddDocuments(string url, string index, string apiKey, List<AzureDocumentApiModel> models);
        Response<SearchResults<T>> SearchDocuments<T>(string url, string index, string apiKey, string query, int rows = 10);
        Response<IndexDocumentsResult> DeleteDocuments(string url, string index, string apiKey, List<string> documentKeys);
        Response<IndexDocumentsResult> DeleteDocumentsByQuery(string url, string index, string apiKey, string filter);
        Response<IndexDocumentsResult> DeleteAllDocuments(string url, string index, string apiKey);
    }

    public class AzureApiService : IAzureApiService
    {
        #region Constructor

        public AzureApiService() { }

        protected SearchClient GetClient(string url, string index, string apiKey)
        {
            //primary admin key
            var uri = new Uri(url);
            var creds = new AzureKeyCredential(apiKey);
            var client = new SearchClient(uri, index, creds);

            return client;
        }

        #endregion

        #region Context
        
        public Response<IndexDocumentsResult> AddDocuments(string url, string index, string apiKey, List<AzureDocumentApiModel> models)
        {
            var client = GetClient(url, index, apiKey);
            var response = client.UploadDocuments(models);
            
            return response;
        }
        
        public Response<IndexDocumentsResult> DeleteDocuments(string url, string index, string apiKey, List<string> documentKeys)
        {
            var client = GetClient(url, index, apiKey);

            var response = client.DeleteDocuments("", documentKeys);

            return response;
        }

        public Response<IndexDocumentsResult> DeleteDocumentsByQuery(string url, string index, string apiKey, string filter)
        {
            var client = GetClient(url, index, apiKey);
            var opt = new SearchOptions 
            { 
                Filter = filter,
                IncludeTotalCount = true 
            };
            
            var results = client.Search<AzureDocumentApiModel>("", opt);
            if (results.Value.TotalCount == 0)
                return null;

            var removeResults = results.Value.GetResults().Select(a => a.Document.id);
            var response = client.DeleteDocuments("id", removeResults);

            return response;
        }

        public Response<IndexDocumentsResult> DeleteAllDocuments(string url, string index, string apiKey)
        {
            var client = GetClient(url, index, apiKey);
            var opt = new SearchOptions { IncludeTotalCount = true };
            var results = client.Search<AzureDocumentApiModel>("", opt);
            if (results.Value.TotalCount == 0)
                return null;

            var removeResults = results.Value.GetResults().Select(a => a.Document.id);
            var response = client.DeleteDocuments("id", new List<string>());

            return response;
        }
        
        public Response<SearchResults<T>> SearchDocuments<T>(string url, string index, string apiKey, string query, int rows = 10)
        {
            var client = GetClient(url, index, apiKey);
            var options = new SearchOptions 
            { 
                Size = rows
            };

            var response = client.Search<T>(query, options);

            return response;
        }
        
        #endregion
    }
}
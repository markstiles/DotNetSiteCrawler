using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SiteIndexer.Services.Solr
{
    public interface ISolrClient
    {
        T SendGet<T>(string apiUrl);
        T SendPost<T>(string apiUrl, object parameter);
        HttpStatusCode SendStatusPost(string apiUrl, object parameter);
        HttpStatusCode SendDelete(string apiUrl);
    }

    public class SolrClient : ISolrClient
    {
        #region Constructor

        protected readonly ISettings Settings;
        protected readonly ILogService LogService;
        protected readonly IApiClient ApiClient;

        protected readonly JsonSerializerSettings SerialSettings;

        public SolrClient(ISettings settings, ILogService logService, IApiClient apiClient)
        {
            Settings = settings;
            LogService = logService;
            ApiClient = apiClient;
            SerialSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
        }

        #endregion
        
        private string JsonContentType => "application/json";

        public T SendGet<T>(string apiUrl)
        {
            LogService.Info($"SolrClient.SendGet - ApiUrl: {apiUrl}");

            ApiClient.InnerClient.DefaultRequestHeaders.Clear();
            using (var res = Task.Run(() => ApiClient.InnerClient.GetAsync(apiUrl)))
            {
                res.Wait();
                var content = res.Result.Content.ReadAsStringAsync().Result;
                LogService.Debug($"SolrClient.SendGet - Response: {content}");
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                var response = JsonConvert.DeserializeObject<T>(content, settings);

                return response;
            }
        }

        public T SendPost<T>(string apiUrl, object parameter)
        {
            var serialContent = JsonConvert.SerializeObject(parameter, SerialSettings);
            LogService.Info($"SolrClient.SendPost - ApiPath: {apiUrl} - Parameter: {serialContent}");

            var paramContent = new StringContent(serialContent, Encoding.UTF8, JsonContentType);
            ApiClient.InnerClient.DefaultRequestHeaders.Clear();
            ApiClient.InnerClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonContentType));
            using (var res = Task.Run(() => ApiClient.InnerClient.PostAsync(apiUrl, paramContent)))
            {
                res.Wait();
                var content = res.Result.Content.ReadAsStringAsync().Result;
                LogService.Debug($"SolrClient.SendPost - Response: {content}");
                var response = JsonConvert.DeserializeObject<T>(content);

                return response;
            }
        }

        public HttpStatusCode SendStatusPost(string apiUrl, object parameter)
        {
            var serialContent = JsonConvert.SerializeObject(parameter, SerialSettings);
            LogService.Info($"SolrClient.SendStatusPost - ApiPath: {apiUrl} - Parameter: {serialContent}");

            var paramContent = new StringContent(serialContent, Encoding.UTF8, JsonContentType);
            ApiClient.InnerClient.DefaultRequestHeaders.Clear();
            ApiClient.InnerClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonContentType));
            using (var res = Task.Run(() => ApiClient.InnerClient.PostAsync(apiUrl, paramContent)))
            {
                res.Wait();
                var status = res.Result.StatusCode;
                LogService.Debug($"SolrClient.SendStatusPost - Response: {status}");

                return status;
            }
        }

        public HttpStatusCode SendDelete(string apiUrl)
        {
            LogService.Info($"SolrClient.SendDelete - ApiPath: {apiUrl}");

            ApiClient.InnerClient.DefaultRequestHeaders.Clear();
            using (var res = Task.Run(() => ApiClient.InnerClient.DeleteAsync(apiUrl)))
            {
                res.Wait();
                var status = res.Result.StatusCode;
                LogService.Debug($"SolrClient.SendDelete - Response: {status}");

                return status;
            }
        }
    }
}

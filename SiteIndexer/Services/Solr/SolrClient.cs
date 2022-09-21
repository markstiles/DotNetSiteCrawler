using Newtonsoft.Json;
using SiteIndexer.Services.System;
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
    }

    public class SolrClient : ISolrClient
    {
        #region Constructor

        protected readonly ISettings Settings;
        protected readonly ILogService LogService;
        protected readonly HttpClient Client;

        protected readonly JsonSerializerSettings SerialSettings;

        public SolrClient(ISettings settings, ILogService logService, IHttpClientFactory clientFactory)
        {
            Settings = settings;
            LogService = logService;
            Client = clientFactory.CreateClient();
            //TODO move this to the config
            Client.BaseAddress = new Uri("http://localhost:900");
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

            Client.DefaultRequestHeaders.Clear();
            using (var res = Task.Run(() => Client.GetAsync(apiUrl)))
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
            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonContentType));
            using (var res = Task.Run(() => Client.PostAsync(apiUrl, paramContent)))
            {
                res.Wait();
                var content = res.Result.Content.ReadAsStringAsync().Result;
                LogService.Debug($"SolrClient.SendPost - Response: {content}");
                var response = JsonConvert.DeserializeObject<T>(content);

                return response;
            }
        }
    }
}

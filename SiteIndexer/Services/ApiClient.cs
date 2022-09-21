using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace SiteIndexer.Services
{
    public interface IApiClient
    {
        HttpClient InnerClient { get; }
    }

    public class ApiClient : IApiClient
    {
        private HttpClient _client;

        public HttpClient InnerClient
        {
            get
            {
                if (_client != null)
                    return _client;

                _client = new HttpClient();

                return _client;
            }
        }
    }
}
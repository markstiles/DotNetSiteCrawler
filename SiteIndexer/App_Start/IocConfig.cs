using SiteIndexer.Form.Attributes;
using SiteIndexer.Services;
using System.Web;
using System.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SiteIndexer.Controllers;
using SiteIndexer.Factories;
using SiteIndexer.Services.Solr;

namespace SiteIndexer
{
    public class IocConfig
    {
        public static void Configure(ServiceCollection services)
        {
            services.AddHttpClient();

            //settings
            services.AddTransient<ISettings, Settings>();

            //controllers
            services.AddTransient<HomeController>();
            services.AddTransient<ConfigurationController>();
            services.AddTransient<IndexingController>();

            //services
            services.AddTransient<ILogService, LogService>();
            services.AddTransient<ICacheService, CacheService>();
            services.AddTransient<ISolrApiService, SolrApiService>();
            services.AddTransient<ISolrClient, SolrClient>();
            services.AddTransient<IApiClient, ApiClient>();
            services.AddTransient<IConfigurationService, ConfigurationService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<ICrawlingService, CrawlingService>();
            
            //factories
            //services.AddTransient<IIssueViewModelFactory, IssueViewModelFactory>();
        }
    }
}

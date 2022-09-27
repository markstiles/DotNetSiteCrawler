using SiteIndexer.Services;
using System.Web;
using System.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SiteIndexer.Controllers;
using SiteIndexer.Factories;
using SiteIndexer.Services.Solr;
using SiteIndexer.Services.System;
using SiteIndexer.Services.Configuration;
using SiteIndexer.Services.Crawling;
using SiteIndexer.Services.Indexing;
using SiteIndexer.Services.Jobs;

namespace SiteIndexer
{
    public class IocConfig
    {
        public static void Configure(ServiceCollection services)
        {
            services.AddHttpClient();

            //controllers
            services.AddTransient<HomeController>();
            services.AddTransient<ConfigurationController>();
            services.AddTransient<IndexingController>();
            services.AddTransient<SearchController>();

            //services
            services.AddTransient<ILogService, LogService>();
            services.AddTransient<ICacheService, CacheService>();
            services.AddTransient<ISolrApiService, SolrApiService>();
            services.AddSingleton<ISolrClient, SolrClient>();
            services.AddTransient<IConfigurationService, ConfigurationService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<ICrawlingService, CrawlingService>();
            services.AddTransient<IIndexingService, IndexingService>();
            services.AddTransient<IStringService, StringService>();
            services.AddSingleton<IJobService, JobService>();

            //factories
            services.AddTransient<ISiteParserFactory, SiteParserFactory>();
        }
    }
}

using Newtonsoft.Json;
using SiteIndexer.Services.Configuration.Models;
using SiteIndexer.Services.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace SiteIndexer.Services.Configuration
{
    public interface IConfigurationService
    {
        SolrConnectionModel GetSolrConnection(Guid id);
        List<SolrConnectionModel> GetSolrConnections();
        SolrConnectionModel CreateSolrConnection(Guid id, string url, string core);
        AzureConnectionModel GetAzureConnection(Guid id);
        List<AzureConnectionModel> GetAzureConnections();
        AzureConnectionModel CreateAzureConnection(Guid id, string url, string core, string apiKey);
        SiteModel GetSite(Guid id);
        List<SiteModel> GetSites();
        SiteModel CreateSite(Guid id, string url, string parser);
        CrawlerModel GetCrawler(Guid id);
        List<CrawlerModel> GetCrawlers();
        CrawlerModel CreateCrawler(Guid id, string crawlerName, Guid connectionId, List<Guid> siteIds, string type);
    }

    public class ConfigurationService : IConfigurationService
    {
        #region Constructor

        public Dictionary<Guid, SolrConnectionModel> SolrConnections { get; set; }
        public Dictionary<Guid, AzureConnectionModel> AzureConnections { get; set; }
        public Dictionary<Guid, SiteModel> Sites { get; set; }
        public Dictionary<Guid, CrawlerModel> Crawlers { get; set; }

        protected IFileService FileService;

        public ConfigurationService(IFileService fileservice)
        {
            FileService = fileservice;
            SolrConnections = new Dictionary<Guid, SolrConnectionModel>();
            AzureConnections = new Dictionary<Guid, AzureConnectionModel>();
            Sites = new Dictionary<Guid, SiteModel>();
            Crawlers = new Dictionary<Guid, CrawlerModel>();

            var solrFiles = FileService.GetFiles("App_Data/configurations/connections");
            foreach(var f in solrFiles)
            {
                if (f.Key.Contains("solr"))
                {
                    var model = JsonConvert.DeserializeObject<SolrConnectionModel>(f.Value);

                    SolrConnections.Add(model.Id, model);
                }
                else if (f.Key.Contains("azure"))
                {
                    var model = JsonConvert.DeserializeObject<AzureConnectionModel>(f.Value);

                    AzureConnections.Add(model.Id, model);
                }                
            }

            var siteFiles = FileService.GetFiles("App_Data/configurations/sites");
            foreach (var f in siteFiles)
            {
                var model = JsonConvert.DeserializeObject<SiteModel>(f.Value);

                Sites.Add(model.Id, model);
            }

            var crawlerFiles = FileService.GetFiles("App_Data/configurations/crawlers");
            foreach (var f in crawlerFiles)
            {
                var model = JsonConvert.DeserializeObject<CrawlerModel>(f.Value);

                Crawlers.Add(model.Id, model);
            }
        }

        #endregion

        public SolrConnectionModel GetSolrConnection(Guid id)
        {
            return SolrConnections.ContainsKey(id) ? SolrConnections[id] : null;
        }

        public List<SolrConnectionModel> GetSolrConnections()
        {
            return SolrConnections.Values.ToList();
        }

        public SolrConnectionModel CreateSolrConnection(Guid id, string url, string core)
        {
            var config = new SolrConnectionModel
            {
                Id = id,
                Url = url,
                Core = core
            };

            var content = JsonConvert.SerializeObject(config);
            FileService.WriteFile($"App_Data/configurations/connections/solr-{id}.json", content);

            return config;
        }

        public AzureConnectionModel GetAzureConnection(Guid id)
        {
            return AzureConnections.ContainsKey(id) ? AzureConnections[id] : null;
        }

        public List<AzureConnectionModel> GetAzureConnections()
        {
            return AzureConnections.Values.ToList();
        }

        public AzureConnectionModel CreateAzureConnection(Guid id, string url, string core, string apiKey)
        {
            var config = new AzureConnectionModel
            {
                Id = id,
                Url = url,
                Core = core,
                ApiKey = apiKey
            };

            var content = JsonConvert.SerializeObject(config);
            FileService.WriteFile($"App_Data/configurations/connections/azure-{id}.json", content);

            return config;
        }

        public SiteModel GetSite(Guid id)
        {
            return Sites[id];
        }

        public List<SiteModel> GetSites()
        {
            return Sites.Values.ToList();
        }

        public SiteModel CreateSite(Guid id, string url, string parser)
        {
            var config = new SiteModel
            {
                Id = id,
                Url = url,
                Parser = parser
            };

            var content = JsonConvert.SerializeObject(config);
            FileService.WriteFile($"App_Data/configurations/sites/{id}.json", content);

            return config;
        }

        public CrawlerModel GetCrawler(Guid id)
        {
            return Crawlers[id];
        }

        public List<CrawlerModel> GetCrawlers()
        {
            return Crawlers.Values.ToList();
        }

        public CrawlerModel CreateCrawler(Guid id, string crawlerName, Guid connectionId, List<Guid> siteIds, string type)
        {
            var config = new CrawlerModel
            {
                Id = id,
                CrawlerName = crawlerName,
                Connection = connectionId,
                Sites = siteIds,
                Type = type
            };

            var content = JsonConvert.SerializeObject(config);
            FileService.WriteFile($"App_Data/configurations/crawlers/{id}.json", content);

            return config;
        }
    }
}
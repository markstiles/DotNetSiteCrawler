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
        SiteModel GetSite(Guid id);
        List<SiteModel> GetSites();
        SiteModel CreateSite(Guid id, string url);
        CrawlerModel GetCrawler(Guid id);
        List<CrawlerModel> GetCrawlers();
        CrawlerModel CreateCrawler(Guid id, string crawlerName, Guid solrConnectionId, List<Guid> siteIds);
    }

    public class ConfigurationService : IConfigurationService
    {
        #region Constructor

        public Dictionary<Guid, SolrConnectionModel> SolrConnections { get; set; }
        public Dictionary<Guid, SiteModel> Sites { get; set; }
        public Dictionary<Guid, CrawlerModel> Crawlers { get; set; }

        protected IFileService FileService;

        public ConfigurationService(IFileService fileservice)
        {
            FileService = fileservice;
            SolrConnections = new Dictionary<Guid, SolrConnectionModel>();
            Sites = new Dictionary<Guid, SiteModel>();
            Crawlers = new Dictionary<Guid, CrawlerModel>();

            var solrFiles = FileService.GetFiles("App_Data/configurations/solr");
            foreach(var f in solrFiles)
            {
                var model = JsonConvert.DeserializeObject<SolrConnectionModel>(f);

                SolrConnections.Add(model.Id, model);
            }

            var siteFiles = FileService.GetFiles("App_Data/configurations/sites");
            foreach (var f in siteFiles)
            {
                var model = JsonConvert.DeserializeObject<SiteModel>(f);

                Sites.Add(model.Id, model);
            }

            var crawlerFiles = FileService.GetFiles("App_Data/configurations/crawlers");
            foreach (var f in crawlerFiles)
            {
                var model = JsonConvert.DeserializeObject<CrawlerModel>(f);

                Crawlers.Add(model.Id, model);
            }
        }

        #endregion

        public SolrConnectionModel GetSolrConnection(Guid id)
        {
            return SolrConnections[id];
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
            FileService.WriteFile($"App_Data/configurations/solr/{id}.json", content);

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

        public SiteModel CreateSite(Guid id, string url)
        {
            var config = new SiteModel
            {
                Id = id,
                Url = url
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

        public CrawlerModel CreateCrawler(Guid id, string crawlerName, Guid solrConnectionId, List<Guid> siteIds)
        {
            var config = new CrawlerModel
            {
                Id = id,
                CrawlerName = crawlerName,
                SolrConnection = solrConnectionId,
                Sites = siteIds
            };

            var content = JsonConvert.SerializeObject(config);
            FileService.WriteFile($"App_Data/configurations/crawlers/{id}.json", content);

            return config;
        }
    }
}
using Newtonsoft.Json;
using SiteIndexer.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace SiteIndexer.Services
{
    public interface IConfigurationService
    {
        Dictionary<string, ConfigurationModel> Configurations { get; set; }

        ConfigurationModel GetConfiguration(string projectName);
        void CreateConfiguration(string projectNam);
    }

    public class ConfigurationService : IConfigurationService
    {
        #region Constructor

        protected IFileService FileService;

        public ConfigurationService(IFileService fileservice)
        {
            FileService = fileservice;
            Configurations = new Dictionary<string, ConfigurationModel>();

            var files = FileService.GetFiles("configurations");
            foreach(var f in files)
            {
                var model = JsonConvert.DeserializeObject<ConfigurationModel>(f);

                Configurations.Add(model.ProjectName, model);
            }
        }

        #endregion

        public Dictionary<string, ConfigurationModel> Configurations { get; set; }

        public ConfigurationModel GetConfiguration(string projectName)
        {
            return Configurations[projectName];
        }

        public void CreateConfiguration(string projectName)
        {
            var config = new ConfigurationModel
            {
                ProjectName = projectName
            };

            var content = JsonConvert.SerializeObject(config);
            FileService.WriteFile($"configurations/{projectName}.json", content);
        }
    }
}
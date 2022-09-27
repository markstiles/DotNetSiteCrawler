using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SiteIndexer.Models.ViewModels;
using SiteIndexer.Services.Indexing.SiteParsers;
using SiteIndexer.Services.Solr.Models;

namespace SiteIndexer.Factories
{
    public interface ISiteParserFactory
    {
        ISiteParser Create(string type);
    }

    public class SiteParserFactory : ISiteParserFactory
    {
        public ISiteParser Create(string type = "Default")
        {
            //TODO create more sophisticated mechanism to create parsing

            if (type == "Default")
                return new DefaultSiteParser();

            return new DefaultSiteParser();
        }
    }
}
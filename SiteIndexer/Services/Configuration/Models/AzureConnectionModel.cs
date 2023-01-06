using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Services.Configuration.Models
{
    public class AzureConnectionModel
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Core { get; set; }
        public string ApiKey { get; set; }
    }
}
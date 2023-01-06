using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;

namespace SiteIndexer.Services.Azure.Models
{
    public class AzureDocumentApiModel
    {
        //"@search.action" - upload (default) | merge | mergeOrUpload | delete
        [JsonPropertyName("@search.action")]
        public string searchaction { get; set; }
        public string id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public DateTime Updated { get; set; }
    }
}

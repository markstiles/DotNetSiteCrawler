using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Services.Azure.Models
{
    public class AzureUpdateRequestApiModel
    {
        public AzureDocumentApiModel[] value { get; set; }
    }
}
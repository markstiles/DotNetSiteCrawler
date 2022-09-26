using SiteIndexer.Models.FormModels.ValidationMessages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SiteIndexer.Models.FormModels
{
    public class SolrConfigFormModel
    {
        [Required(ErrorMessage = SolrConfigMessages.SolrUrlRequired)]
        public string SolrUrl { get; set; }
        [Required(ErrorMessage = SolrConfigMessages.SolrCoreRequired)]
        public string SolrCore { get; set; }
    }
}
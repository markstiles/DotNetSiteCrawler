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
        [Required(ErrorMessage = ConfigMessages.SolrUrlRequired)]
        public string SolrUrl { get; set; }
        [Required(ErrorMessage = ConfigMessages.SolrCoreRequired)]
        public string SolrCore { get; set; }
    }
}
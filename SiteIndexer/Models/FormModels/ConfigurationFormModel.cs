using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SiteIndexer.Form.ValidationMessages;
using SiteIndexer.Form.Validators;

namespace SiteIndexer.Models.FormModels
{
    public class ConfigurationFormModel
    {
        [Required(ErrorMessage = ConfigMessages.ConfigurationRequired)]
        public string configuration { get; set; }
    }
}
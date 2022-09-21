using SiteIndexer.Form.ValidationMessages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SiteIndexer.Models.FormModels
{
    public class CreateConfigurationFormModel
    {
        [Required(ErrorMessage = ConfigMessages.ProjectNameRequired)]
        public string ProjectName { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SiteIndexer.Models;
using SiteIndexer.Services.System;
using Newtonsoft.Json;

namespace SiteIndexer.Form.Attributes
{
    public class ValidateForm : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var LogService = new LogService();

            var errors = new List<string>();

            var inputParams = actionContext.ActionParameters;
            foreach (var value in inputParams)
            {
                var validationContext = new ValidationContext(value.Value, null, null);
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(value.Value, validationContext, validationResults, true);
                if (!isValid)
                    errors.AddRange(validationResults.Select(a => a.ErrorMessage));
            }

            if(!errors.Any())
                return;

            var result = new TransactionResult
            {
                Succeeded = false,
                ErrorMessage = string.Join(", ", errors)
            };

            LogService.Error($"ValidateForm.OnActionExecuting - result: {JsonConvert.SerializeObject(result)}");

            actionContext.Result = new JsonResult
            {
                Data = result
            };
        }
    }
}
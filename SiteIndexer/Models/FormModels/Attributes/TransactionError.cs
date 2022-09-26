using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Filters;
using System.Web.Mvc;
using SiteIndexer.Models;
using SiteIndexer.Services.System;
using Newtonsoft.Json;

namespace SiteIndexer.Models.FormModels.Attributes
{
    public class TransactionError : IExceptionFilter
    {
        protected ILogService Log;

        public TransactionError()
        {
            
        }

        public void OnException(ExceptionContext filterContext)
        {
            Log = DependencyResolver.Current.GetService<ILogService>();
            Log.Error($"RouteObject: {JsonConvert.SerializeObject(filterContext.RouteData.Values)}");
            var formValues = filterContext.HttpContext.Request.Form.AllKeys.Select(a => $"{a}:{filterContext.HttpContext.Request.Form[a]}");
            Log.Error($"FormValues: {string.Join(", ", formValues)}");
            var qsValues = filterContext.HttpContext.Request.QueryString.AllKeys.Select(a => $"{a}:{filterContext.HttpContext.Request.QueryString[a]}");
            Log.Error($"QueryString: {string.Join(", ", qsValues)}");
            Log.Error("", filterContext.Exception);

            filterContext.ExceptionHandled = true;
            filterContext.Result = new JsonResult {
                Data = new TransactionResult
                {
                    Succeeded = false,
                    ErrorMessage = filterContext.Exception.InnerException?.InnerException?.Message 
                    ?? filterContext.Exception.InnerException?.Message 
                    ?? filterContext.Exception.Message
                }
            };
        }
    }
}
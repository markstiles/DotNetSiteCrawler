using SiteIndexer.Form.Attributes;
using SiteIndexer.Services;
using System.Web;
using System.Web.Mvc;

namespace SiteIndexer
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new TransactionError());
            filters.Add(new HandleErrorAttribute());
        }
    }
}

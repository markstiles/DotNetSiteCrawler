using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;

namespace SiteIndexer.Services.System
{
    public interface IStringService
    {
        string GetValidKey(Uri uri);
    }

    public class StringService : IStringService
    {
        public string GetValidKey(Uri uri)
        {
            return HttpUtility.UrlDecode(uri.AbsoluteUri)
                .Replace($"{uri.Scheme}://", "")
                .Replace("www.", "")
                .Replace(".", "-")
                .Replace("/", "_")
                .Replace("?", "_")
                .Replace("&", "_")
                .Replace(" ", "_");
        }
    }
}
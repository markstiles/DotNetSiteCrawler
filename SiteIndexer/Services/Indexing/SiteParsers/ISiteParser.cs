using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Services.Indexing.SiteParsers
{
    public interface ISiteParser
    {
        List<string> GetAllowedPageExtensions();
        string GetTitle(HtmlDocument html);
        string GetContent(HtmlDocument html);
    }
}
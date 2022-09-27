using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SiteIndexer.Services.Indexing.SiteParsers
{
    public class DefaultSiteParser : ISiteParser
    {
        public List<string> GetAllowedPageExtensions()
        {
            var allowedExtensions = new List<string>
            {
                "php", "htm", "html", "jsp", "asp", "aspx"
            };

            return allowedExtensions;
        }

        public string GetTitle(HtmlDocument html)
        {
            var metatags = html.DocumentNode.SelectNodes("//meta");
            //TODO build title with fallbacks to og title or meta title or h1
            var title = html.DocumentNode.SelectSingleNode("/html/head/title");

            return title?.InnerText ?? "";
        }

        public string GetContent(HtmlDocument html)
        {
            var body = html.DocumentNode.SelectSingleNode("//body");
            var bodyText = "";
            if (body != null)
            {
                var nodesToRemove = body.SelectNodes("//a").ToList();
                foreach (var node in nodesToRemove)
                    node.Remove();

                var bodyInnerText = string.Join(" ", body.InnerText.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries));

                var charArr = bodyInnerText.ToCharArray();
                StringBuilder sb = new StringBuilder();
                foreach (char c in charArr)
                {
                    if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
                        continue;

                    sb.Append(c);
                }

                bodyText = string.Join(" ", sb.ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
            }

            return bodyText;
        }
    }
}
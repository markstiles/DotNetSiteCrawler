using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace SiteIndexer.Services
{
    public interface ICrawlingService
    {
        //string GetFile(string filePath);
    }

    public class CrawlingService : ICrawlingService
    {
        //public string GetFile(string filePath)
        //{
        //    var newPath = CleanFileName(filePath);
        //    var fullFilePath = $"{AppContext.BaseDirectory}/{newPath}";
        //    string text = File.ReadAllText(fullFilePath);

        //    return text;
        //}
    }
}
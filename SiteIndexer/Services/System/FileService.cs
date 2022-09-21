using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace SiteIndexer.Services.System
{
    public interface IFileService
    {
        string GetFile(string filePath);
        List<string> GetFiles(string directoryPath);
        void WriteFile(string filePath, string content);
        void EnsureFolderExists(string filePath, bool isDirectory = true);
        string CleanFileName(string filePath);
    }

    public class FileService : IFileService
    {
        public string GetFile(string filePath)
        {
            var newPath = CleanFileName(filePath);
            var fullFilePath = $"{AppContext.BaseDirectory}/{newPath}";
            string text = File.ReadAllText(fullFilePath);

            return text;
        }

        public List<string> GetFiles(string directoryPath)
        {
            var fileList = new List<string>();
            if (string.IsNullOrWhiteSpace(directoryPath))
                return fileList;

            var folderPath = $"{AppContext.BaseDirectory}/{directoryPath}";
            EnsureFolderExists(folderPath);
            DirectoryInfo di = new DirectoryInfo(folderPath);
            FileInfo[] rgFiles = di.GetFiles("*.json");

            foreach (FileInfo fi in rgFiles)
            {
                string text = File.ReadAllText(fi.FullName);
                fileList.Add(text);
            }

            return fileList;
        }

        public void WriteFile(string filePath, string content)
        {
            EnsureFolderExists(filePath, false);
            var newPath = CleanFileName(filePath);
            var fullFilePath = $"{AppContext.BaseDirectory}/{newPath}";
            File.WriteAllText(fullFilePath, content);
        }

        public void EnsureFolderExists(string filePath, bool isDirectory = true)
        {
            //make a folder based on the domain and subfolders
            List<string> folders = filePath.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if(!isDirectory)
                folders.RemoveAt(folders.Count - 1);

            //make sure the directory exists
            StringBuilder dirPath = new StringBuilder();
            foreach (string f in folders)
            {
                dirPath.Append(f + @"\");
                try
                {
                    DirectoryInfo fd = new DirectoryInfo(dirPath.ToString());
                    if (!fd.Exists)
                        Directory.CreateDirectory(dirPath.ToString());
                }
                catch (Exception ex) { }
            }
        }

        public string CleanFileName(string filePath)
        {
            string path = filePath.Replace("/", @"\").Replace(" ", "").Replace(@"\", "\\");
            string invalid = new string(Path.GetInvalidPathChars());
            //strip bad chars
            foreach (char c in invalid)
                if (path.Contains(c))
                    path = path.Replace(c.ToString(), "");

            return path.ToLower();
        }
    }
}
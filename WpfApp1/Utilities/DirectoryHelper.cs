using System;
using System.IO;
using System.Xml;

namespace WpfApp1.Utilities
{
    internal static class DirectoryHelper
    {
        internal static string GetInputFolderPath()
        {
            var folderPath = ReadConfigFile("input");
            CreateFolderIfNotExists(folderPath);
            return folderPath;
        }

        internal static string GetOutputFolderPath()
        {
            var folderPath = ReadConfigFile("output");
            return folderPath;
        }

        private static void CreateFolderIfNotExists(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Console.WriteLine("Folder created: " + folderPath);
            }
            else
            {
                Console.WriteLine("Folder already exists: " + folderPath);
            }
        }

        private static string ReadConfigFile(string configXPath)
        {
            var files = Directory.GetFiles("./config");
            var fileContent = File.ReadAllText(files[0]);

            var root = new XmlDocument();
            root.LoadXml(fileContent);
            var node = root.SelectSingleNode("descendant::config/" + configXPath);
            return node.InnerText;


        }


    }
}

using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace WpfApp1.Utilities
{
    internal class DirectoryHelper
    {
        internal static string GetInputFolderPath()
        {
            var folderPath = ReadConfigFile("input");
            //var folderPath = GenerateFileFolder();
            CreateFolderIfNotExists(folderPath);
            return folderPath;
        }

        private static string GenerateFileFolder()
        {
            var directoryRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var folderPath = Path.Combine(directoryRoot, "File");
            return folderPath;
        }

        internal static string GetOutputFolderPath()
        {
            var folderPath = ReadConfigFile("output");
            //var folderPath = GenerateFileFolder();
            //var outputFolderPath = Path.Combine(folderPath, "Out");
            //CreateFolderIfNotExists(folderPath);
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

            // come riusare CreateXml?
            XmlDocument root = new XmlDocument();
            root.LoadXml(fileContent);
            var node = root.SelectSingleNode("descendant::config/" + configXPath);
            return node.InnerText;


        }


    }
}

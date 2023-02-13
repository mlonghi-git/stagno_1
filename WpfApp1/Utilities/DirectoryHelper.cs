using System;
using System.IO;
using System.Reflection;

namespace WpfApp1.Utilities
{
    internal class DirectoryHelper
    {
        internal static string GetInputFolderPath()
        {
            var folderPath = GenerateFileFolder();
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
            var folderPath = GenerateFileFolder();
            var outputFolderPath = Path.Combine(folderPath, "Out");
            CreateFolderIfNotExists(outputFolderPath);
            return outputFolderPath;
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
    }
}

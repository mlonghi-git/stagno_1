using System;
using System.IO;
using System.Reflection;

namespace WpfApp1.Utilities
{
    internal class DirectoryHelper
    {
        internal static string GetFolderPath()
        {
            var directoryRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var folderPath = Path.Combine(directoryRoot, "File");
            CreateFolderIfNotExists(folderPath);
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
    }
}

using FluentColorConsole;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace CleanerLite
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            if (SystemFactory.GetOSPlatform() == OSPlatform.Windows)
            {
                if (!SystemFactory.IsCurrentProcessAdmin())
                {
                    ColorConsole.WithYellowText.WriteLine("This software is not running as administrator, so it might NOT delete specific files/folders.");
                }

                var builder = new ConfigurationBuilder().AddJsonFile("appSettings.json", true, true);

                var config = builder.Build();
                var paths = config.GetSection("Paths").GetChildren().Where(x => !string.IsNullOrWhiteSpace(x.Value)).Select(x => x.Value).ToList();

                foreach (var path in paths)
                {
                    DeleteFilesAndFolders(path);
                }
            }
            else
            {
                throw new Exception("This software only runs on Windows.");
            }

            ColorConsole.WithGreenText.WriteLine("Job done!");
            Console.ReadKey();
        }

        private static void DeleteFilesAndFolders(string path)
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                string[] folders = Directory.GetDirectories(path);

                DeleteFiles(files);
                DeleteFolders(folders);
            }
        }

        private static bool IsDirectoryEmpty(string folderPath)
        {
            if (!string.IsNullOrWhiteSpace(folderPath))
            {
                return !Directory.EnumerateFileSystemEntries(folderPath).Any();
            }

            return false;
        }

        private static void DeleteFolders(string[] folderPaths)
        {
            foreach (var folder in folderPaths)
            {
                try
                {
                    DeleteFilesAndFolders(folder);

                    Directory.Delete(folder, true);
                    ColorConsole.WithGreenText.WriteLine($"{folder} deleted.");
                }
                catch (Exception ex)
                {
                    ColorConsole.WithRedText.WriteLine($"Could not delete {folder}: {ex.Message}.");
                }
            }
        }

        private static void DeleteFiles(string[] files)
        {
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                    ColorConsole.WithGreenText.WriteLine($"{file} deleted.");
                }
                catch (Exception ex)
                {
                    ColorConsole.WithRedText.WriteLine($"Could not delete {file}: {ex.Message}.");
                }
            }
        }
    }
}

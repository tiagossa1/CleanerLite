using FluentColorConsole;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using CleanerLite.Services;

namespace CleanerLite
{
    internal static class Program
    {
        // This should be initialized by DI, but the project is so small that it's not worth it.
        private static readonly DeleteFileService DeleteFileService = new();
        private static readonly DeleteFolderService DeleteFolderService = new();
        
        private static bool HasBeenRunAsAdmin => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        
        private static void Main(string[] args)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new NotSupportedException("This software only runs on Windows.");
            }
            
            if (!HasBeenRunAsAdmin)
            {
                ColorConsole.WithYellowText.WriteLine("This software is not running as administrator, so it might NOT delete specific files/folders.");
            }

            var config = GetConfiguration();
            var paths = config
                .GetSection("Paths")
                .Get<List<string>>()
                .FindAll(path => !string.IsNullOrWhiteSpace(path));

            foreach (var path in paths)
            {
                DeleteFilesAndFolders(path);
            }

            ColorConsole.WithGreenText.WriteLine("All paths have been cleaned.");
            Console.ReadKey();
        }

        private static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appSettings.json", true, true);
            return builder.Build();
        }

        private static void DeleteFilesAndFolders(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }
            
            var files = Directory.GetFiles(path);
            var folders = Directory.GetDirectories(path);
            
            DeleteFolderService.Delete(folders);
            DeleteFileService.Delete(files);
        }
    }
}

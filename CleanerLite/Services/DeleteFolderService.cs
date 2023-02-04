using System;
using System.Collections.Generic;
using System.IO;
using FluentColorConsole;

namespace CleanerLite.Services;

public class DeleteFolderService
{
    public void Delete(IEnumerable<string> folderPaths)
    {
        foreach (var folder in folderPaths)
        {
            try
            {
                Directory.Delete(folder, true);
                ColorConsole.WithGreenText.WriteLine($"{folder} deleted.");
            }
            catch (Exception ex)
            {
                ColorConsole.WithRedText.WriteLine($"Could not delete {folder}: {ex.Message}.");
            }
        }
    }
}
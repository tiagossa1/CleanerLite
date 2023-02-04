using System;
using System.Collections.Generic;
using System.IO;
using FluentColorConsole;

namespace CleanerLite.Services;

public class DeleteFileService
{
    public void Delete(IEnumerable<string> files)
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
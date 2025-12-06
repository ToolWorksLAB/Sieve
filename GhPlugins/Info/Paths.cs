using Eto.Forms;
using Sieve.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sieve.Info
{
    public class Paths
    {
        public static string GhEnvFolder =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sieve");

        public static string ScanFilePath => Path.Combine(GhEnvFolder, "scan.json");
        public static string CustomPath => Path.Combine(GhEnvFolder, "CustomPath.json");
    }
    public class Tools
    {
        public static void SaveScan(List<PluginItem> scanData)
        {
            Directory.CreateDirectory(Paths.GhEnvFolder); // safe even if it exists

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(scanData, options);
            File.WriteAllText(Paths.ScanFilePath, json);

        }


        public static List<Models.PluginItem> LoadScan()
        {
            if (!File.Exists(Paths.ScanFilePath))
                return new List<Models.PluginItem>();

            string json = File.ReadAllText(Paths.ScanFilePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<Models.PluginItem>>(json, options)
                   ?? new List<Models.PluginItem>();
        }

        public static void AppendPathToJson(string newPath)
        {
            if (string.IsNullOrWhiteSpace(newPath))
                return;

            if (string.IsNullOrWhiteSpace(Paths.CustomPath))
                return; // caller didn’t set it, so do nothing

            try
            {
                var filePath = Paths.CustomPath;

                // Read existing list (if any)
                List<string> paths;
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        paths = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
                    }
                    else
                    {
                        paths = new List<string>();
                    }
                }
                else
                {
                    paths = new List<string>();
                }

                // Avoid duplicates (case-insensitive)
                bool alreadyExists = paths.Any(p =>
                    string.Equals(p, newPath, StringComparison.OrdinalIgnoreCase));

                if (!alreadyExists)
                {
                    paths.Add(newPath);

                    // Ensure directory exists
                    var dir = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    // Save back to JSON (pretty printed)
                    var jsonOut = JsonSerializer.Serialize(
                        paths,
                        new JsonSerializerOptions { WriteIndented = true }
                    );
                    File.WriteAllText(filePath, jsonOut);
                }
            }
            catch (Exception ex)
            {
                // You can remove this if you don't want UI errors
                MessageBox.Show(
                    "Could not save the new path:\n" + ex.Message,
                    "Error saving path",
                    MessageBoxType.Error);
            }


        }

        public static List<string> LoadCustomPaths()
        {
            // Always return a valid list, never null
            var result = new List<string>();

            if (string.IsNullOrWhiteSpace(Paths.CustomPath))
                return result;

            try
            {
                if (!File.Exists(Paths.CustomPath))
                    return result;

                var json = File.ReadAllText(Paths.CustomPath);
                if (string.IsNullOrWhiteSpace(json))
                    return result;

                var paths = JsonSerializer.Deserialize<List<string>>(json);
                if (paths == null)
                    return result;

                // Clean up: trim, drop empties, remove duplicates (case-insensitive)
                result = paths
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(p => p.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                return result;
            }
            catch
            {
                // If anything goes wrong (invalid JSON, IO error, etc.),
                // just return an empty list. Optionally log the exception.
                return new List<string>();
            }
        }
    }
}


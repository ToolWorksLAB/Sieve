using Eto.Forms;
using Sieve.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
            if (scanData == null)
                return;

            // Ensure folder exists
            Directory.CreateDirectory(Paths.GhEnvFolder);

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(scanData, settings);
            File.WriteAllText(Paths.ScanFilePath, json);
        }

        public static List<Models.PluginItem> LoadScan()
        {
            try
            {
                if (!File.Exists(Paths.ScanFilePath))
                    return new List<Models.PluginItem>();

                string json = File.ReadAllText(Paths.ScanFilePath);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<Models.PluginItem>();

                var items = JsonConvert.DeserializeObject<List<Models.PluginItem>>(json);
                return items ?? new List<Models.PluginItem>();
            }
            catch
            {
                // On any error, just return empty list
                return new List<Models.PluginItem>();
            }
        }

        public static void AppendPathToJson(string newPath)
        {
            if (string.IsNullOrWhiteSpace(newPath))
                return;

            if (string.IsNullOrWhiteSpace(Paths.CustomPath))
                return; // no target path defined

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
                        paths = JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
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
                    var jsonOut = JsonConvert.SerializeObject(
                        paths,
                        new JsonSerializerSettings { Formatting = Formatting.Indented }
                    );
                    File.WriteAllText(filePath, jsonOut);
                }
            }
            catch (Exception ex)
            {
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

                var paths = JsonConvert.DeserializeObject<List<string>>(json);
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
                // just return an empty list.
                return new List<string>();
            }
        }
    }
}

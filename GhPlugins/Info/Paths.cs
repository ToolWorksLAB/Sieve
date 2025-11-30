using GhPlugins.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GhPlugins.Info
{
    public class Paths
    {
        public static string GhEnvFolder =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ghEnv");

        public static string ScanFilePath => Path.Combine(GhEnvFolder, "scan.json");
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
    }
}

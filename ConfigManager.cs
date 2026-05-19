using System;
using System.IO;
using System.Text.Json;

namespace SkiddingApp
{
    public sealed class AppConfig
    {
        public bool AimbotEnabled { get; set; } = false;
        public string AimbotKey { get; set; } = "Mouse1";
        public float AimbotFov { get; set; } = 120f;
        public float AimbotSmoothness { get; set; } = 8f;

        public bool Use3DTargeting { get; set; } = false;

        public bool TriggerbotEnabled { get; set; } = false;
        public string TriggerbotKey { get; set; } = "Mouse2";
        public float TriggerbotRadius { get; set; } = 32f;

        public bool SpeedEnabled { get; set; } = false;
        public float SpeedValue { get; set; } = 32f;

        public bool ChamsEnabled { get; set; } = false;

        public bool ESPMasterEnabled { get; set; } = true;
        public bool TeamCheckEnabled { get; set; } = true;
        public bool KnockCheckEnabled { get; set; } = false;
        public bool BoxesEnabled { get; set; } = true;
        public bool FillBackground { get; set; } = false;
        public bool HealthBarEnabled { get; set; } = true;
        public bool NamesEnabled { get; set; } = true;
        public string NameType { get; set; } = "Username";
        public bool DistanceEnabled { get; set; } = true;
    }

    public static class ConfigManager
    {
        private static readonly string ConfigFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LumenSkid");
        private static readonly string ConfigFile = Path.Combine(ConfigFolder, "config.json");

        public static string ConfigFilePath => ConfigFile;

        public static AppConfig Load()
        {
            try
            {
                if (!Directory.Exists(ConfigFolder))
                    Directory.CreateDirectory(ConfigFolder);

                if (File.Exists(ConfigFile))
                {
                    string json = File.ReadAllText(ConfigFile);
                    var config = JsonSerializer.Deserialize<AppConfig>(json);
                    if (config != null)
                        return config;
                }
            }
            catch { }

            return new AppConfig();
        }

        public static void Save(AppConfig config)
        {
            try
            {
                if (!Directory.Exists(ConfigFolder))
                    Directory.CreateDirectory(ConfigFolder);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(config, options);
                File.WriteAllText(ConfigFile, json);
            }
            catch { }
        }
    }
}

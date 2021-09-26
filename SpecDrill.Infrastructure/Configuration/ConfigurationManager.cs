using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpecDrill.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace SpecDrill.Infrastructure.Configuration
{
    public class ConfigurationManager
    {
        private const string ConfigurationFileName = "specDrillConfig.json";
        protected static readonly ILogger Logger;

        public static readonly Settings Settings;
        static ConfigurationManager()
        {
            Logger = DI.GetLogger<ConfigurationManager>();
            Settings = Load();
        }

        public static Settings Load(string? jsonConfiguration = null, string? configurationFileName = null)
        {
            if (string.IsNullOrWhiteSpace(jsonConfiguration))
            {
                Logger.LogInformation($"Searching Configuration file {configurationFileName??ConfigurationFileName}...");
                var configurationPaths = FindConfigurationFile(AppDomain.CurrentDomain.BaseDirectory, configurationFileName ?? ConfigurationFileName);

                if (configurationPaths == ("", ""))
                    throw new FileNotFoundException("Configuration file not found");

                var configurationFilePath = configurationPaths.Item1;
                var jsonConfigurationFilePath = configurationPaths.Item2;

                if (string.IsNullOrWhiteSpace(jsonConfigurationFilePath))
                {
                    Logger.LogInformation("Configuration file not found.");
                    throw new FileNotFoundException("Configuration file not found");
                }

                IConfigurationBuilder configBuilder = new ConfigurationBuilder().AddJsonFile(jsonConfigurationFilePath,false, false);

                IConfigurationRoot? configRoot = configBuilder.Build();
                if (configRoot == null)
                {
                    Logger.LogInformation("Configuration file not found.");
                    throw new FileNotFoundException("Configuration file not found");
                }
                DI.AddConfiguration(configRoot.GetSection("webdriver"));
                DI.Apply();

                jsonConfiguration = File.ReadAllText(jsonConfigurationFilePath);
            }
            if (jsonConfiguration == null) throw new InvalidDataException("jsonConfiguration not provided or could not be read from configuration file!");

            Settings? configuration = JsonSerializer.Deserialize<Settings>(
                jsonConfiguration,
                new JsonSerializerOptions()
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true
                });

            if (configuration == null) throw new InvalidDataException("jsonConfiguration could not be deserialized!");

            return configuration;
        }

        private static (string folder, string result) FindConfigurationFile(string folder, string configurationFileName)
        {
            while (true)
            {
                Logger.LogInformation($"Scanning {folder}...");

                // we need at least a valid root folder path to continue
                if (folder.Length > 2)
                {
                    var result = Directory.EnumerateFiles(folder, "*.json", SearchOption.TopDirectoryOnly).FirstOrDefault(file => file.ToLowerInvariant().EndsWith(configurationFileName.ToLowerInvariant()));

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        Logger.LogInformation($"Found configuration file at {result}");
                        return (folder, result);
                    }

                    folder = GetParentFolder(folder);
                    continue;
                }

                return ("", "");
            }
        }

        private static string GetParentFolder(string folder)
        {
            return folder.Remove(folder.LastIndexOf('\\'));
        }
    }
}

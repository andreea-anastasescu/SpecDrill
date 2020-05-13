using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Repository;
using Newtonsoft.Json;
using SpecDrill.Configuration;
using SpecDrill.Infrastructure.Logging;

namespace SpecDrill.Infrastructure.Configuration
{
    public class ConfigurationManager
    {
        private const string ConfigurationFileName = "specDrillConfig.json";
        protected static readonly Logging.Interfaces.ILogger Log;

        public static readonly Settings Settings;
        static ConfigurationManager()
        {
            Settings = Load();
            Log = Logging.Log.Get<ConfigurationManager>();
        }

        public static Settings Load(string jsonConfiguration = null)
        {
            if (string.IsNullOrWhiteSpace(jsonConfiguration))
            {
                Log.Info($"Searching Configuration file {ConfigurationFileName}...");
                var configurationPaths = FindConfigurationFile(AppDomain.CurrentDomain.BaseDirectory);
                
                if (configurationPaths == null)
                    throw new FileNotFoundException("Configuration file not found");

                var configurationFilePath = configurationPaths.Item1;
                var log4netConfigFilePath = Path.Combine(configurationFilePath, "log4net.config");

                var logRepository = LogManager.GetRepository(Assembly.GetCallingAssembly());
                var log4NetConfig = new FileInfo(log4netConfigFilePath);

                XmlConfigurator.Configure(logRepository, log4NetConfig);

                var jsonConfigurationFilePath = configurationPaths.Item2;
                
                if (string.IsNullOrWhiteSpace(jsonConfigurationFilePath))
                {
                    Log.Info("Configuration file not found.");
                    throw new FileNotFoundException("Configuration file not found");
                }

                jsonConfiguration = File.ReadAllText(jsonConfigurationFilePath);
            }
            var configuration =  JsonConvert.DeserializeObject<Settings>(jsonConfiguration);

            //var configuration = new Configuration();
            //configuration.Selenium = new SeleniumConfiguration();
            return configuration;
        }

        private static Tuple<string, string> FindConfigurationFile(string folder)
        {
            while (true)
            {
                Log.Info($"Scanning {folder}...");

                // we need at least a valid root folder path to continue
                if (folder.Length > 2)
                {
                    var result = Directory.EnumerateFiles(folder, "*.json", SearchOption.TopDirectoryOnly).FirstOrDefault(file => file.ToLowerInvariant().EndsWith(ConfigurationFileName.ToLowerInvariant()));

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        Log.Info($"Found configuration file at {result}");
                        return Tuple.Create(folder, result);
                    }

                    folder = GetParentFolder(folder);
                    continue;
                }
             
                return null;
            }
        }

        private static string GetParentFolder(string folder)
        {
            return folder.Remove(folder.LastIndexOf('\\'));
        }
    }
}

﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpecDrill.Configuration.WebDriver;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace SpecDrill.Infrastructure.Configuration
{
    public sealed class ConfigurationManager
    {
        private const string ConfigurationFileName = "specDrillConfig.json";
        private static readonly ILogger Logger;
       
        public static readonly SpecDrill.Configuration.Settings Settings;

        static ConfigurationManager()
        {
            Logger = DI.GetLogger<ConfigurationManager>();
            Settings = Load();
        }
        public static SpecDrill.Configuration.Settings Load(string? jsonConfiguration = null, string? configurationFileName = null, string? baseDirectory = null)
        {
            IConfigurationRoot? configRoot;
            if (string.IsNullOrWhiteSpace(jsonConfiguration))
            {
                var cfgFileName = configurationFileName ?? ConfigurationFileName;
                var cfgBaseDirectory = baseDirectory ?? AppDomain.CurrentDomain.BaseDirectory;
                Logger.LogInformation($"Searching Configuration file {cfgFileName}...");
                var configurationPaths = FindConfigurationFile(cfgBaseDirectory, cfgFileName);

                if (configurationPaths == ("", ""))
                    throw new FileNotFoundException("Configuration file not found");

                var (_, jsonConfigurationFullFilePath) = configurationPaths;

                if (string.IsNullOrWhiteSpace(jsonConfigurationFullFilePath))
                {
                    Logger.LogInformation("Configuration file not found.");
                    throw new FileNotFoundException("Configuration file not found");
                }

                Logger.LogInformation($"Configuring json fine -> path = {jsonConfigurationFullFilePath}");
                configRoot = new ConfigurationBuilder()
                    .AddJsonFile(jsonConfigurationFullFilePath, false, false)
                    .Build();
            }
            else
            {
                // this is for unit testing scenarios. reset is necessary for clearing the DI ServiceCollecton.
                // it cannot be called from the unit test since first call to Load(...) is from our static constructor.
                DI.Reset(apply: true);
                var configStream = new MemoryStream(Encoding.ASCII.GetBytes(jsonConfiguration));
                configRoot = new ConfigurationBuilder()
                    .AddJsonStream(configStream)
                    .Build();
            }
            
            if (configRoot == null) throw new InvalidDataException("jsonConfiguration not provided or could not be read from configuration file!");
            Logger.LogInformation("DI.AddConfiguration(configRoot);");
            DI.AddConfiguration(configRoot);
            DI.Apply();
            IOptions<SpecDrill.Configuration.Settings>? wdc = DI.ServiceProvider.GetService<IOptions<SpecDrill.Configuration.Settings>>();

            if (wdc == null) throw new Exception("DI could not resolve IOptions<Settings>");

            //new JsonSerializerOptions()
            //{
            //    ReadCommentHandling = JsonCommentHandling.Skip,
            //    AllowTrailingCommas = true,
            //    PropertyNameCaseInsensitive = true
            //}

            return wdc.Value;
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
            Logger.LogInformation($"folder = {folder}; directorySeparatorChar = {Path.DirectorySeparatorChar}");
            return folder.Remove(folder.LastIndexOf(Path.DirectorySeparatorChar));
        }
    }
}

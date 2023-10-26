using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SpecDrill.Configuration;
using SpecDrill.Configuration.WebDriver;
using System;

namespace SpecDrill.Infrastructure
{
    public static class String { }
    public static class DI
    {
        private static Func<IServiceCollection> DefaultServiceCollection
                                            = () => new ServiceCollection()
                                                            .AddLogging(
                                                                builder => builder
                                                                            .SetMinimumLevel(LogLevel.Information)
                                                                            .AddConsole()
                                                                            .AddFile("app.log", append: true)
                                                                                        
                                                                            //.AddFile("c:\\apps\\app_{0:yyyy}-{0:MM}-{0:dd}.log", fileLoggerOpts => {
                                                                            //    fileLoggerOpts.FormatLogFileName = fName => String.Format(fName, DateTime.UtcNow);
                                                                            //})
                                                                );
        private static IServiceCollection serviceCollection = DefaultServiceCollection();

        private static Lazy<ServiceProvider> serviceProvider = ReDeploy();

        public static ServiceProvider ServiceProvider => serviceProvider.Value;
        private static readonly ILoggerFactory LoggerFactory = DI.ServiceProvider.GetService<ILoggerFactory>()
                                ?? throw new ArgumentNullException(nameof(LoggerFactory));
        public static ILogger GetLogger<T>()
            => LoggerFactory.CreateLogger<T>();

        public static ILogger GetLogger(Type type)
            => LoggerFactory.CreateLogger(type);

        #region Modifying operations on ServiceCollection
        public static void AddConfiguration(IConfiguration configuration)
            => serviceCollection.Configure<Settings>(configuration);
        
        public static Lazy<ServiceProvider> ReDeploy()
            => new(() => serviceCollection.BuildServiceProvider());
        public static Lazy<ServiceProvider> Apply()
            => serviceProvider = ReDeploy();
        public static void Reset(bool apply = false)
        {
            serviceCollection = DefaultServiceCollection();
            if (apply)
                Apply();
        }

        public static void ConfigureServices(Action<IServiceCollection> configureServices)
        {
            /*serviceCollection = */configureServices(serviceCollection);
        }
        #endregion
    }
}

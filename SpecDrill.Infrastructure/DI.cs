using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SpecDrill.Configuration;
using System;

namespace SpecDrill.Infrastructure
{
    public class DI
    {
        private static readonly IServiceCollection serviceCollection = new ServiceCollection()
                                                            .AddLogging(
                                                                builder => builder
                                                                            .SetMinimumLevel(LogLevel.Information)
                                                                            .AddConsole());

        private static Lazy<ServiceProvider> serviceProvider = RefreshServiceProvider();
            
        private static Lazy<ServiceProvider> RefreshServiceProvider()
            => new Lazy<ServiceProvider>(() => serviceCollection.BuildServiceProvider());
        
        public static ServiceProvider ServiceProvider => serviceProvider.Value;
        private static readonly ILoggerFactory LoggerFactory = DI.ServiceProvider.GetService<ILoggerFactory>()
                                ?? throw new ArgumentNullException(nameof(LoggerFactory));
        public static ILogger GetLogger<T>()
            => LoggerFactory.CreateLogger<T>();

        public static ILogger GetLogger(Type type)
            => LoggerFactory.CreateLogger(type);

        #region Modifying operations on ServiceCollection
        public static void AddConfiguration(IConfiguration configuration)
            => serviceCollection.Configure<WebDriverConfiguration>(configuration.GetSection("webdriver"));
        
        public static void Apply() => serviceProvider = RefreshServiceProvider();
        #endregion
    }
}

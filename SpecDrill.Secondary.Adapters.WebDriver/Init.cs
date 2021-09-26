using Microsoft.Extensions.DependencyInjection;
using SpecDrill.Configuration;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Secondary.Ports.AutomationFramework.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpecDrill.Secondary.Adapters.WebDriver
{
    public static class Init
    {
        internal class RuntimeServices : IRuntimeServices
        {
            public RuntimeServices(IElementLocatorFactory locatorFactory)
                => ElementLocatorFactory = locatorFactory;
            public IBrowserDriverFactory GetBrowserFactoryBuilder(Settings settings) => new SeleniumBrowserFactory(settings);

            public IElementLocatorFactory ElementLocatorFactory { get; private set; }

            public IElementFactory GetElementFactory(IBrowser browser) => new ElementFactory(browser);
        }
        
        public static void AddWebdriverSecondaryAdapter(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IElementLocatorFactory, ElementLocatorFactory>();
            serviceCollection.AddScoped<IRuntimeServices, RuntimeServices>();
        }
    }
}

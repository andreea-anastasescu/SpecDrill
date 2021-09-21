using SpecDrill.Configuration;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Secondary.Ports.AutomationFramework.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpecDrill.Secondary.Adapters.WebDriver
{
    public static class Runtime
    {
        internal class RuntimeServices : IRuntimeServices
        {
            public RuntimeServices(IElementLocatorFactory locatorFactory)
                => ElementLocatorFactory = locatorFactory;
            public IBrowserDriverFactory GetBrowserFactoryBuilder(Settings settings) => new SeleniumBrowserFactory(settings);

            public IElementLocatorFactory ElementLocatorFactory { get; private set; }

            public IElementFactory GetElementFactory(IBrowser browser) => new ElementFactory(browser);
        }
        public static IRuntimeServices GetServices()
            => new RuntimeServices(new ElementLocatorFactory());
    }
}

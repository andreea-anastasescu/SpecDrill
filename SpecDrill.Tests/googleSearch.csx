#r "nuget: SpecDrill, 1.3.0.648"
#r "nuget: SpecDrill.Secondary.Adapters.WebDriver, 1.3.0.704"

using System;
using SpecDrill;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Secondary.Ports.AutomationFramework.Core;
using Microsoft.Extensions.DependencyInjection;
using SpecDrill.Secondary.Adapters.WebDriver;
using SpecDrill.Infrastructure;
using SpecDrill.Infrastructure.Configuration;
using SpecDrill.Configuration;

public class SearchResultItemControl : WebControl
{
    [Find(By.XPath, "//div/div/div[1]/a/h3")]
    public IElement Link { get; private set; }

    [Find(By.XPath, "//div[1]/div[1]/div[2]/div[1]")]
    public IElement Description { get; private set; }

    public SearchResultItemControl(IElement parent, IElementLocator locator) : base(parent, locator) { }
}

public class GoogleSearchResultsPage : GoogleSearchPage
{
    [Find(By.CssSelector, "div#search div.g")]
    public ListElement<SearchResultItemControl> SearchResults { get; private set; }
}

public class GoogleSearchPage : WebPage
{
    [Find(By.XPath, "//input[@name='q']")]
    public IElement TxtSearch { get; private set; }

    [Find(By.XPath, "//div[contains(@class,'FPdoLc')]//input[@name='btnK']")]
    public INavigationElement<GoogleSearchResultsPage> BtnSearch { get; private set; }
}

DI.ConfigureServices(
    services => 
    {
        services.AddWebdriverSecondaryAdapter();
        services.AddSingleton<Settings>(
            (_) => 
            {
                //Console.WriteLine($"{Args[0]} ... {System.Reflection.Assembly.GetExecutingAssembly().Location}");
                ConfigurationManager.Load(null, null, @"C:\Users\csontu\source\repos\SpecDrill\SpecDrill.Tests");
                return ConfigurationManager.Settings;
            }
        );
        services.AddTransient<IBrowser, Browser>();
    }
);

DI.Apply();
var browser = DI.ServiceProvider.GetService<IBrowser>();
// var googleSearchPage = browser.Open<GoogleSearchPage>();
// var acceptButton = new Element(null, ElementLocatorFactory.Create(By.XPath, "/html/body/div[2]/div[2]/div[3]/span/div/div/div/div[3]/div[1]/button[2]"));

// Wait.NoMoreThan(TimeSpan.FromSeconds(21)).Until(() => acceptButton.IsAvailable,throwExceptionOnTimeout: false);

// googleSearchPage.TxtSearch.SendKeys("drill wiki");
// //googleSearchPage.TxtSearch.Blur();
// Wait.Until(() =>
//     googleSearchPage.BtnSearch.IsDisplayed
// );
// var resultsPage = googleSearchPage.BtnSearch.Click();

// #region Option 1: assuming it's first result
// //resultsPage.SearchResults[1].Link.Text.Should().Contain("Drill");
// #endregion

// #region Option 2: searching through search results
// var found = false;
// foreach (var searchResult in resultsPage.SearchResults)
//     if (searchResult.Link.Text.Contains("Drill"))
//         found = true;
// #endregion


var googleSearchPage = browser.Open<GoogleSearchPage>();
var acceptButton = new Element(null, ElementLocatorFactory.Create(By.Id, "L2AGLb"));
Wait.NoMoreThan(TimeSpan.FromSeconds(21))
    .Until(() => acceptButton.IsAvailable,throwExceptionOnTimeout: false);
if (acceptButton.IsAvailable)
    acceptButton.Click();
googleSearchPage.TxtSearch.SendKeys("drill wiki");
googleSearchPage.TxtSearch.Blur();
Wait.Until(() =>
    googleSearchPage.BtnSearch.IsDisplayed
);
var resultsPage = googleSearchPage.BtnSearch.Click();
var found = resultsPage.SearchResults
                .FirstOrDefault(r => r.Link.Text.Contains("Drill")) != default;

Console.WriteLine($"Rezultatul wikipedia {(found ? string.Empty : "nu")} a fost gasit!");

Console.ReadLine();
browser.Exit();
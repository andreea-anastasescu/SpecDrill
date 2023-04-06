using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecDrill;
using SpecDrill.MsTest;
using SpecDrill.Secondary.Ports.AutomationFramework;
using System;
using System.Reflection.Emit;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using iText.Layout.Splitting;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using iText.StyledXmlParser.Css.Selector;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using SpecDrill.PageObjectModel;
using static SpecDrill.PageObjectModel.PomExtensions;
using static SpecDrill.PageObjectModel.WebPageExtensions;
namespace SomeTests;

[TestClass]
public class CreatingPomClassesAtRuntimeTests : MsTestBase
{
    
    [ClassInitialize]
    public static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);
    public static object syncroot = new();
    [TestMethod]
    //[Ignore("Google search and search results page changed. New selectors have to be applied. Postponed for later.")]

    public void ShouldHaveWikipediaAmongResultsOnGoogleSearch()
    {
        Browser.GoToUrl("https://www.google.com");
        var sitemap = SiteMap("GoogleWebsite", "v1.0", new(), new())
                        .AddComponents(
                            Component("SearchResultItem", null,
                                Element("link", "Link", new PomLocator("xpath", "//div/div/div[1]/a/h3")),
                                Element("link", "Description", new PomLocator("xpath", "//div[1]/div[1]/div[2]/div[1]"))
                            )
                        )
                        .AddPages(
                            Page("GoogleSearchResults", null, null,
                                   ComponentListElement("SearchResults", new PomLocator("cssselector", "div#search div.g"), itemType: "SearchResultItem")
                                ),
                            Page("GoogleSearch", "https://www.google.com", null,
                                Element("button", "BtnAccept", new PomLocator("id", "L2AGLb")),
                                Element("input", "TxtSearch", new PomLocator("xpath", "//input[@name='q'] | //*[@id=\"APjFqb\"]")),
                                Element("button", "BtnSearch", new PomLocator("xpath", "//div[contains(@class,'FPdoLc')]//input[@name='btnK']"), targetPage: "GoogleSearchResults")
                                )
                        );
        // write json in eyebot form

        string fileName = "sitemap.json";
        string jsonString = JsonSerializer.Serialize(sitemap);
        File.WriteAllText(fileName, jsonString);
        
        // process sitemap elements
        var processedComponents = sitemap.components
                                            .Select(c => c with { elements = c.elements.Select(e => (e, t: EyebotTypeToSpecDrillType(e.type, e.itemType, e.targetPage)))
                                            .Select(se => se.e with { type = se.t.type, itemType = se.t.itemType, targetPage = se.t.targetPage}).ToList() })
                                            .ToList();
        var processedPages = sitemap.pages
                                        .Select(c => c with { elements = c.elements.Select(e => (e, t: EyebotTypeToSpecDrillType(e.type, e.itemType, e.targetPage)))
                                        .Select(se => se.e with { type = se.t.type, itemType = se.t.itemType, targetPage = se.t.targetPage }).ToList() })
                                        .ToList();
        // replace original elements with specdrill type names (via EyebotTypeToSpecDrillType(...) transform
        sitemap = sitemap with { components = processedComponents ?? new(), pages = processedPages ?? new() };

        // create POM types !
        sitemap.BuildComponents()
        .BuildPages();

        var googleSearchPageType = sitemap.GetTypeOf("GoogleSearch");
        //var gspType = mb.GetType("GoogleSearch");
        //var gsp = Browser.Open(googleSearchPageType);
        //var googleSearchPageType = sitemap.GetTypeOfPage("GoogleSearch");
        //var gspType = mb.GetType("GoogleSearch");
        var googleSearchPage = Browser.Open(googleSearchPageType);
        //var googleSearchPage = browser.Open<GoogleSearchPage>();

        var acceptButton = googleSearchPage.Property<IElement>(googleSearchPageType, "BtnAccept") ?? throw new Exception($"BtnAccept not accessible on {googleSearchPageType}");
        Wait.NoMoreThan(TimeSpan.FromSeconds(21))
            .Until(() => acceptButton.IsAvailable, throwExceptionOnTimeout: false);
        if (acceptButton.IsAvailable)
            acceptButton.Click();
        var txtSearch = googleSearchPage.Property<IElement>(googleSearchPageType, "TxtSearch");
        Assert.IsNotNull(txtSearch);
        txtSearch.SendKeys("drill wiki");
        txtSearch.Blur();
        var btnSearch = googleSearchPage.Property<INavigationElement<WebPage>>(googleSearchPageType, "BtnSearch");
        Assert.IsNotNull(btnSearch);
        Wait.Until(() =>
            btnSearch.IsDisplayed
        );
        var resultsPage = btnSearch.Click();
        var googleSearchResultsPageType = sitemap.GetTypeOf("GoogleSearchResults");
        var searchResultItemType = sitemap.GetTypeOf("SearchResultItem");
        var found = (resultsPage.Property(googleSearchResultsPageType, "SearchResults") as IEnumerable<WebControl>)?
                        .FirstOrDefault(r => r.Property<Element>(searchResultItemType, "Link")?.Text.Contains("Drill") ?? false) != default;

        Assert.IsTrue(found);

       //Browser.Exit();
    }

    private (string type, string? itemType, string? targetPage) EyebotTypeToSpecDrillType(string type, string? itemType, string? targetPage)
    => (type, itemType, targetPage) switch
    {
        ("button", null, null) => ("element", null, null),
        ("link", null, null) => ("element", null, null),
        ("input", null, null) => ("element", null, null),
        ("button", null, var tPage) => ("navigation_element", null, tPage),
        ("link", null, var tPage) => ("navigation_element", null, tPage),
        ("list", var iType, null) => ("list", iType, null),
        ("select", null, null) => ("select_element", null, null),
        (var t, var iT, var tP) => throw new Exception($"Unexpected combination: type={t}, itemType={iT}, targetPage={tP} !")
    };
}

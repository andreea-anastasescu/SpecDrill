using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecDrill;
using SpecDrill.MsTest;
using SpecDrill.Secondary.Ports.AutomationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;
using SpecDrill.PageObjectModel;
using static SpecDrill.PageObjectModel.PomExtensions;
using static SpecDrill.PageObjectModel.WebPageExtensions;
using System.Collections.Immutable;

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

        var sitemap = SiteMap("GoogleWebsite1", "v1.0", new(), new())
                        .AddComponents(
                            Component("SearchResultItem", null,
                                Element("link", "Link", new PomLocator("xpath", "./div/div/div/div[1]/div/a | ./div/div[1]/div/a")),
                                Element("label", "Description", new PomLocator("xpath", "./div/div/div/div[2]/div/span | ./div/div[2]/div/span"))
                            )
                        )
                        .AddPages(
                            Page("GoogleSearchResults", null,
                                   ComponentListElement("SearchResults", new PomLocator("xpath", "//*[@id=\"rso\"]/div/div[1]/div[1]/div[starts-with(@class,\"g \") or @class = \"g\"] | //*[@id=\"rso\"]/div/div[starts-with(@class,\"g \") or @class = \"g\"] | //*[@id=\"rso\"]/div/div/div[starts-with(@class,\"g \") or @class = \"g\"]"), itemType: "SearchResultItem")
                                ),
                            Page("GoogleSearch", null,
                                Element("button", "BtnAccept", new PomLocator("id", "L2AGLb")),
                                Element("input", "TxtSearch", new PomLocator("xpath", "//input[@name='q'] | //*[@id=\"APjFqb\"]")),
                                Element("button", "BtnSearch", new PomLocator("xpath", "//div[contains(@class,'FPdoLc')]//input[@name='btnK']"), targetPage: "GoogleSearchResults")
                                )
                        );
        // write json in random form
        //var jsonInput = File.ReadAllText("C:\\_apps\\sitemap.json");
        //var sitemap = JsonSerializer.Deserialize<PomSitemap>(jsonInput);
        //Assert.IsNotNull(sitemap);

        string fileName = "sitemap.json";
        string jsonString = JsonSerializer.Serialize(sitemap);
       // jsonString.Replace("components", "sections");
        File.WriteAllText(fileName, jsonString);

        // READ FILE VERSION
        var jsonInput = File.ReadAllText("C:\\_apps\\sitemap.json");
        sitemap = JsonSerializer.Deserialize<PomSitemap>(jsonInput);
        Assert.IsNotNull(sitemap);

        // process sitemap elements
        sitemap = ConvertToSpecDrillTypes(sitemap);

        


        // create POM types !
        sitemap.Build();

        var googleSearchPageType = sitemap.GetTypeOf("GoogleSearch");
        //var gspType = mb.GetType("GoogleSearch");
        //var gsp = Browser.Open(googleSearchPageType);
        //var googleSearchPageType = sitemap.GetTypeOfPage("GoogleSearch");
        //var gspType = mb.GetType("GoogleSearch");
        var googleSearchPage = Browser.Open(googleSearchPageType, "https://www.google.com");
        //var googleSearchPage = Browser.Open(googleSearchPageType);
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
        var results = (resultsPage.Property(googleSearchResultsPageType, "SearchResults") as IListElement<WebControl>);

        foreach (var r in results)
        {
            var lnk = ((IElement)searchResultItemType.GetProperty("Link").GetValue(r)).Text;
            var descr = ((IElement)searchResultItemType.GetProperty("Description").GetValue(r)).Text;
        }
        var found = results?
                        .FirstOrDefault(r => r.Property<Element>(searchResultItemType, "Link")?.Text.Contains("Drill") ?? false) != default;

        Assert.IsTrue(found);

        //Browser.Exit();
    }

    private PomSitemap ConvertToSpecDrillTypes(PomSitemap sitemap)
    {
        var processedComponents = sitemap.components
                                                    .Select(c => c with
                                                    {
                                                        elements = c.elements.Select(e => (e, t: ToSpecDrillType(e.type, e.itemType, e.targetPage)))
                                                    .Select(se => se.e with { type = se.t.type, itemType = se.t.itemType, targetPage = se.t.targetPage }).ToList()
                                                    })
                                                    .ToList();
        var processedPages = sitemap.pages
                                        .Select(c => c with
                                        {
                                            elements = c.elements.Select(e => (e, t: ToSpecDrillType(e.type, e.itemType, e.targetPage)))
                                        .Select(se => se.e with { type = se.t.type, itemType = se.t.itemType, targetPage = se.t.targetPage }).ToList()
                                        })
                                        .ToList();
        // replace original elements with specdrill type names (via EyebotTypeToSpecDrillType(...) transform
        sitemap = sitemap with { components = processedComponents ?? new(), pages = processedPages ?? new() };
        return sitemap;
    }

    [TestMethod]
    public void GetDeclarationOrderBasedOnDependencies()
    {
        var sitemap = SiteMap("GoogleWebsite", "v1.1", new(), new())
                        .AddComponents(
                            Component("SearchResultItem", null,
                                Element("link", "Link", new PomLocator("xpath", "./div/div/div/div[1]/div/a | ./div/div[1]/div/a")),
                                Element("label", "Description", new PomLocator("xpath", "./div/div/div/div[2]/div/span | ./div/div[2]/div/span"))
                            )
                        )
                        .AddPages(
                            Page("GoogleSearchResults", null,
                                   ComponentListElement("SearchResults", new PomLocator("xpath", "//*[@id=\"rso\"]/div/div[1]/div[1]/div[starts-with(@class,\"g \") or @class = \"g\"] | //*[@id=\"rso\"]/div/div[starts-with(@class,\"g \") or @class = \"g\"] | //*[@id=\"rso\"]/div/div/div[starts-with(@class,\"g \") or @class = \"g\"]"), itemType: "SearchResultItem")
                                ),
                            Page("GoogleSearch", null,
                                Element("button", "BtnAccept", new PomLocator("id", "L2AGLb")),
                                Element("input", "TxtSearch", new PomLocator("xpath", "//input[@name='q'] | //*[@id=\"APjFqb\"]")),
                                Element("button", "BtnSearch", new PomLocator("xpath", "//div[contains(@class,'FPdoLc')]//input[@name='btnK']"), targetPage: "GoogleSearchResults")
                                )
                        );
        // process sitemap elements
        sitemap = ConvertToSpecDrillTypes(sitemap);


        //var decl_ord = sitemap.GetDeclarationOrder();

        sitemap.Build();

    }
    [TestMethod]
    public void MoreComplexSitemapDefinition_DeclarationOrderTest_ShouldPass()
    {
        var sitemap = SiteMap("EwaWebsite", "v1.0", new(), new()).AddComponents(Component("PersonResultItem", null, Element("label", "PersonLabel", new PomLocator("xpath", "./span/div[1]//label")), Element("button", "PersonItem", new PomLocator("xpath", "."), targetPage: "PersonDetailsPage"))).AddPages(Page("PersonDetailsPage", null, Element("label", "PersonName", new PomLocator("xpath", "//*[@id=\"formHeaderTitle_1\"]")), Element("label", "Discipline", new PomLocator("xpath", "//div[@id='tab-section3']//div[@role='tabpanel']/section[1]/section[1]/div[@role='presentation']/div[@role='presentation']/div[2]/div[2]/div[@role='presentation']/div/div[@role='presentation']/div[@role='presentation']/div[3]/div[@role='presentation']/div[@role='presentation']/div[2]/div[1]/div[@role='presentation']/div[@role='presentation']/div[1]//div[@role='link']/div[@role='presentation']"))), Page("EwaHomePage", null, Element("input", "TxtSearch", new PomLocator("xpath", "/html//input[@id='GlobalSearchBox']")), ComponentListElement("PersonResults", new PomLocator("xpath", "//*[@id=\"id-globalSearchFlyout-1\"]/div/div/div/div/div/button"), itemType: "PersonResultItem")), Page("PortalSelectionPage", null, Element("label", "Header", new PomLocator("xpath", "//*[@id=\"app-section-header_1\"]")), Element("button", "TileEwa", new PomLocator("xpath", """//*[@id="AppModuleTileSec_1_Item_2"]"""), targetPage: "EwaHomePage")), Page("CrmLandingPage", null, Element("label", "TopBar", new PomLocator("xpath", "//*[@id=\"topBar\"]")), Element("frame", "PortalSelectionFrame", new PomLocator("xpath", """//*[@id="AppLandingPage"]"""), targetPage: "PortalSelectionPage")), Page("StaySignedInPage", null, Element("input", "LblStaySignedIn", new PomLocator("xpath", "/html//div[@id='lightbox']/div[@role='main']//div[@role='heading']")), Element("button", "BtnYes", new PomLocator("xpath", "/html//input[@id='idSIButton9']"), targetPage: "CrmLandingPage")), Page("Login2Page", null, Element("input", "TxtPassword", new PomLocator("xpath", "/html//div[@id='lightbox']/div[@role='main']/div/div[2]/div//input[@name='passwd']")), Element("button", "BtnSignIn", new PomLocator("xpath", "/html//input[@id='idSIButton9']"), targetPage: "StaySignedInPage")), Page("Login1Page", null, Element("button", "BtnAccept", new PomLocator("id", "L2AGLb")), Element("input", "TxtUsername", new PomLocator("xpath", "/html//div[@id='lightbox']/div[@role='main']/div/div/div//input[@name='loginfmt']")), Element("button", "BtnNext", new PomLocator("xpath", "/html//input[@id='idSIButton9']"), targetPage: "Login2Page")));

        sitemap = ConvertToSpecDrillTypes(sitemap);
        

        sitemap.Build();
    }
    private (string type, string? itemType, string? targetPage) ToSpecDrillType(string type, string? itemType, string? targetPage)
    => (type.ToLowerInvariant(), itemType, targetPage) switch
    {
        ("button", null, null) => ("element", null, null),
        ("label", null, null) => ("element", null, null),
        ("link", null, null) => ("element", null, null),
        ("input", null, null) => ("element", null, null),
        ("button", null, var tPage) => ("navigation_element", null, tPage),
        ("link", null, var tPage) => ("navigation_element", null, tPage),
        ("frame", null, var tPage) => ("frame_element", null, tPage),
        ("list", var iType, null) => ("list", iType, null),
        ("select", null, null) => ("select_element", null, null),

        ("element", null, null) => ("element", null, null),
        ("navigation_element", null, var tPage) => ("navigation_element", null, tPage),
        ("frame_element", null, var tPage) => ("frame_element", null, tPage),
        //("list", var iType, null) => ("list", iType, null),
        ("select_element", null, null) => ("select_element", null, null),
        (null, null, null) => ("page", null, null),
        (var t, var iT, var tP) => throw new Exception($"Unexpected combination: type={t}, itemType={iT}, targetPage={tP} !")
    };
}

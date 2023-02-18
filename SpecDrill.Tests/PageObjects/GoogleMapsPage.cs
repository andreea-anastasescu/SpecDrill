using SpecDrill;
using SpecDrill.Secondary.Ports.AutomationFramework;

#nullable disable

namespace SomeTests.PageObjects
{
    public class GoogleMapsPage : WebPage
    {
       [Find(By.CssSelector, "#scene>div.widget-scene>canvas")]
       public IElement MapCanvas { get; set; }
    }
}

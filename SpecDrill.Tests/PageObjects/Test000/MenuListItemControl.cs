using SpecDrill;
using SpecDrill.Secondary.Ports.AutomationFramework;

#nullable disable

namespace SomeTests.PageObjects.Test000
{
    public class MenuListItemControl : WebControl
    {
        public MenuListItemControl(IElement parent, IElementLocator locator) : base(parent, locator)
        {
            //LnkLogin = WebElement.CreateNavigation<Test000LoginPage>(parent, ElementLocator.Create(By.PartialLinkText, "Login"));
            //LnkHome = WebElement.CreateNavigation<Test000HomePage>(parent, ElementLocator.Create(By.PartialLinkText, "Home"));
        }
        [Find(By.PartialLinkText, "Login")]
        public INavigationElement<Test000LoginPage> LnkLogin { get; private set; }
        [Find(By.PartialLinkText, "Home")]
        public INavigationElement<Test000HomePage> LnkHome { get; private set; }
    }
}

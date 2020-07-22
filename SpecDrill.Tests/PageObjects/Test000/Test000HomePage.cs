using SpecDrill;
using SpecDrill.SecondaryPorts.AutomationFramework;

namespace SomeTests.PageObjects.Test000
{
    public class Test000HomePage : WebPage
    {
        //[Find(By.Id, "username")]
        public IElement LblUserName { get; private set; }

        //[Find(By.Id, "login")]
        public INavigationElement<Test000HomePage> BtnLogin { get; private set; }
        //[Find(By.Id, "menu")]
        public MenuListItemControl CtlMenu { get; private set; }

        public Test000HomePage()
            : base("Virtual Store - Home")
        {
            this.LblUserName = WebElement.Create(this, ElementLocator.Create(By.Id, "username"));
            this.BtnLogin = WebElement.CreateNavigation<Test000HomePage>(this, ElementLocator.Create(By.Id, "login"));
            this.CtlMenu = WebElement.CreateControl<MenuListItemControl>(this, ElementLocator.Create(By.Id, "menu"));
        }
    }
}

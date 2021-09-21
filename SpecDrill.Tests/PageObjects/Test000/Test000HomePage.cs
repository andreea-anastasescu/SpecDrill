using SpecDrill;
using SpecDrill.Secondary.Ports.AutomationFramework;

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
            this.LblUserName = ElementFactory.Create(this, ElementLocatorFactory.Create(By.Id, "username"));
            this.BtnLogin = ElementFactory.CreateNavigation<Test000HomePage>(this, ElementLocatorFactory.Create(By.Id, "login"), null); // reconsider nullability of targetLocator
            this.CtlMenu = ElementFactory.CreateControl<MenuListItemControl>(this, ElementLocatorFactory.Create(By.Id, "menu"));
        }
    }
}

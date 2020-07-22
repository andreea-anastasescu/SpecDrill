using SpecDrill;
using SpecDrill.SecondaryPorts.AutomationFramework;

namespace SomeTests.PageObjects.Test000
{
    public class Test000LoginPage : WebPage
    {
        [Find(By.Id, "userName")]
        public IElement TxtUserName { get; private set; }

        [Find(By.Id, "password")]
        public IElement TxtPassword { get; private set; }

        [Find(By.Id, "login")]
        public INavigationElement<Test000HomePage> BtnLogin { get; private set; }

        [Find(By.Id, "country")]
        public ISelectElement DdlCountry { get; private set; }

        [Find(By.Id, "city")]
        public ISelectElement DdlCity { get; private set; }

        [Find(By.Id, "menu")]
        public MenuListItemControl CtlMenu { get; private set; }

        //public Test000LoginPage()
        //    : base("Virtual Store - Login")
        //{

        //}
    }
}

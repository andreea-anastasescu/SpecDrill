using SpecDrill;
using SpecDrill.Secondary.Ports.AutomationFramework;

#nullable disable

namespace SomeTests.PageObjects.Test000
{
    public class MenuControl : WebControl
    {
        [Find(By.Id, "userName")]
        public IElement TxtUserName { get; private set; }
        [Find(By.Id, "password")]
        public IElement TxtPassword { get; private set; }
        [Find(By.Id, "login")]
        public INavigationElement<Test000HomePage> BtnLogin { get; private set; }

        public MenuControl(IElement parent, IElementLocator locator)
            : base(parent, locator)
        {
            //this.TxtUserName = WebElement.Create(this, ElementLocator.Create(By.Id, "userName"));
            //this.TxtPassword = WebElement.Create(this, ElementLocator.Create(By.Id, "password"));
            //this.BtnLogin = WebElement.CreateNavigation<Test000HomePage>(this, ElementLocator.Create(By.Id, "login"));
        }
    }
}

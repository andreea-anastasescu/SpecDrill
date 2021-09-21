using SomeTests.PageObjects.Test002;
using SpecDrill;
using SpecDrill.Secondary.Ports.AutomationFramework;

namespace SomeTests.PageObjects
{
    public class WorkspacePage : WebPage
    {
        private const string PAGE_TITLE = "Workspace";

        [Find(By.CssSelector, "div#workspace_product_menu")]
        public INavigationElement<WorkspacePage> ProductMenu { get; private set; }

        [Find(By.CssSelector, "li.workspace-tab span.tab-title-inner")]
        public ListElement<WebControl> Tabs { get; private set; }

        //[Find(By.XPath, "/html/body/div[5]/div[2]/ul/li")]
        [Find(By.CssSelector, "#menu_sb_MyProducts > li")]
        public ListElement<WebControl> Products { get; private set; }

        [Find(By.CssSelector, "h2.h2.accent--title")]
        public IElement InvoiceManagementTableTitle { get; private set; }
        
        [Find(By.CssSelector, "div#workspace_product_menu")]
        public IElement Btn9dot{ get; set; }    

        //[Find(By.CssSelector, "select[name='Invoice run']")]
        //public ISelectElement InvoiceRunDropDown { get; private set; }

        //[Find(By.CssSelector, "select[name='Paid Status']")]
        //public ISelectElement PaidStatusDropDown { get; private set; }

        //[Find(By.CssSelector, "table[role='table']")]
        //public TableControl Table { get; private set; }

        public void Navigate(string[] menuItems)
        {
            Browser.Click(ElementFactory.Create(null, ElementLocatorFactory.Create(By.CssSelector, $"tag3-side-nav-item[indent='1'][text='{menuItems[0]}']")));

            Browser.Click(ElementFactory.Create(null, ElementLocatorFactory.Create(By.CssSelector, $"tag3-side-nav-item[indent='1'][text='{menuItems[0]}'] tag3-side-nav-item[indent='2'][text='{menuItems[1]}']")));

            Browser.Click(ElementFactory.Create(null, ElementLocatorFactory.Create(By.CssSelector, $"tag3-side-nav-item[indent='1'][text='{menuItems[0]}'] tag3-side-nav-item[indent='2'][text='{menuItems[1]}'] tag3-side-nav-item[indent='3'][text='{menuItems[2]}']")));

        }
        public WorkspacePage() : base(PAGE_TITLE)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SpecDrill;
using SpecDrill.SecondaryPorts.AutomationFramework;

namespace $rootnamespace$.PageObjects
{
    public class GoogleSearchPage : WebPage
    {
        [Find(By.XPath, "/html//form[@id='tsf']/div/div/div/div/div/input[@role='combobox']")]
        public IElement TxtSearch { get; private set; }

        [Find(By.XPath, "/html//form[@id='tsf']/div/div/div/center/input[@name='btnK']")]
        public INavigationElement<GoogleSearchResultsPage> BtnSearch { get; private set; }
    }
}

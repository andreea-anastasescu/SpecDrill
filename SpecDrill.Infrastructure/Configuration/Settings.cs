using SpecDrill.Configuration.Homepages;

namespace SpecDrill.Configuration
{
    public class Settings
    {
        public WebDriver.WebDriverConfiguration? WebDriver { get; set; }

        public HomepageConfiguration[] Homepages { get; set; } = new HomepageConfiguration[0];
    }
}

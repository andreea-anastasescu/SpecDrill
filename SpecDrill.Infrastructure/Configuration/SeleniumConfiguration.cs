namespace SpecDrill.Configuration
{
    public class WebDriverConfiguration
    {
        public string? BrowserDriversPath { get; set; }

        public string? BrowserDriver { get; set; }
        
        public bool? IsRemote { get; set; }
        public string? SeleniumServerUri { get; set; }
    }
}

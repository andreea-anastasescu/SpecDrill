using SpecDrill.Configuration;
using SpecDrill.Infrastructure.Configuration;

namespace SpecDrill.Infrastructure
{
    public static class Globals
    {
        static Globals()
        {
            Configuration = ConfigurationManager.Settings;
        }

        public static Settings Configuration { get; set; }
    }
}

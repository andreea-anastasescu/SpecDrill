using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecDrill.Configuration.Homepages
{
    public class HomepageConfiguration
    {
        public string? PageObjectType { get; set; }

        public string? Url { get; set; }

        public bool IsFileSystemPath { get; set; }
    }
}

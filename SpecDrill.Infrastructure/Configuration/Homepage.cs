using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecDrill.Configuration
{
    public class Homepage
    {
        public string? PageObjectType { get; set; }

        public string? Url { get; set; }

        public bool IsFileSystemPath { get; set; }
    }
}

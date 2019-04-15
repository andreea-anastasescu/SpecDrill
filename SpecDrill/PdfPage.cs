using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SpecDrill.AutomationScopes;
using SpecDrill.Infrastructure.Logging;
using SpecDrill.Infrastructure.Logging.Interfaces;
using SpecDrill.SecondaryPorts.AutomationFramework;
using SpecDrill.SecondaryPorts.AutomationFramework.Core;
using SpecDrill.SecondaryPorts.AutomationFramework.Model;

namespace SpecDrill
{
    public class PdfPage : WebPage
    {
        private string titlePattern;

        public PdfPage() : this(string.Empty) { }
        public PdfPage(string titlePattern) : base(titlePattern)
        {
            this.titlePattern = titlePattern;
        }

        public override string Text
        {
            get
            {
                return Browser.GetPdfText();
            }
        }
    }
}

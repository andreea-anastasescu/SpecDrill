using OpenQA.Selenium;
using SpecDrill.SecondaryPorts.AutomationFramework;
using SpecDrill.SecondaryPorts.AutomationFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecDrill.SecondaryPorts.Adapters.WebDriver
{
    internal class SeleniumAlert : IBrowserAlert
    {
        private Func<IAlert?> alert;
        public SeleniumAlert(Func<IAlert?> alert)
        {
            this.alert = alert;
        }

        public string Text => this.alert()?.Text ?? "";

        public void Accept()
        {
            this.alert()?.Accept();
            Wait.Until(() => alert() == null);
        }

        public void Dismiss()
        {
            this.alert()?.Dismiss();
            Wait.Until(() => alert() == null);
        }

        public void SendKeys(string keys) => alert()?.SendKeys(keys);
    }
}

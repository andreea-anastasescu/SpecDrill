using SpecDrill.Secondary.Ports.AutomationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecDrill.Secondary.Adapters.WebDriver
{
    internal class SeleniumElementLocator : IElementLocator
    {
        private readonly By locatorType;
        private readonly string locatorValue;
        private readonly bool isShadowRoot;
        private readonly int? index; //1-based

        public SeleniumElementLocator(By locatorKind, string locatorValue, bool isShadowRoot = false)
            => (this.locatorType, this.locatorValue, this.index, this.isShadowRoot) = (locatorKind, locatorValue, null, isShadowRoot);

        public SeleniumElementLocator(By locatorKind, string locatorValue, int index, bool isShadowRoot = false) : this(locatorKind, locatorValue, isShadowRoot)
        {
            this.index = index;
        }

        internal static IElementLocator Create(By locatorKind, string locatorValue, bool isShadowRoot = false)
            => new SeleniumElementLocator(locatorKind, locatorValue, isShadowRoot);
        public override string ToString()
        {
            return $"By: {locatorType} -> `{locatorValue}` { (isShadowRoot ? "!" : "") }"; 
        }

        public IElementLocator Copy()
        {
            return new SeleniumElementLocator(this.locatorType, this.LocatorValue, this.IsShadowRoot);
        }
        
        public IElementLocator CopyWithIndex(int index, bool isShadowRoot = false)
        {
            if (index < 1)
                throw new IndexOutOfRangeException("SpecDrill: SeleniumElementLocator.CopyWithIndex(idx) index is 1-based!");

            return new SeleniumElementLocator(this.locatorType, this.LocatorValue, index, isShadowRoot);
        }

        /// <summary>
        /// Creates new Locator by Appending index information to current Locator
        /// </summary>
        /// <param name="index"> index is one based! </param>
        /// <returns></returns>
        //public IElementLocator WithIndex(int index)
        //{
        //    if (index < 1)
        //        throw new ArgumentException("Locator Index is 1-based");

        //    switch (this.locatorType)
        //    {
        //        case By.CssSelector:
        //            return new SeleniumElementLocator(this.LocatorType, $"{this.locatorValue}:nth-of-type({index})");
        //        case By.XPath:
        //            return new SeleniumElementLocator(this.LocatorType, $"{this.locatorValue}[{index}]");
        //        default:
        //            throw new Exception("SpecDrill Invalid Locator Type. You can index only CSS or XPath selectors!");
        //    }
        //}

        public bool IsShadowRoot => isShadowRoot;
        public By LocatorType
        {
            get { return locatorType; }
        }

        public string LocatorValue
        {
            get { return locatorValue; }
        }

        public int? Index
        {
            get
            {
                return index;
            }
        }
    }
}

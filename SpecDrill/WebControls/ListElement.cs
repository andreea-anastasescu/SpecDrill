using SpecDrill.SecondaryPorts.AutomationFramework;
using System;
using System.Collections.Generic;
using SpecDrill.SecondaryPorts.AutomationFramework.Core;
using System.Collections;
using System.Text.RegularExpressions;
using System.Linq;

namespace SpecDrill.WebControls
{
    public interface IListElement<T> : IReadOnlyList<T> 
        where T : class, IElement
    {
        T GetElementByText(string regex);
    }
    public class ListElement<T> : WebControl, IListElement<T>
        where T : /*WebControl,*/ class, IElement
    {
        public ListElement(IElement? parent, IElementLocator locator) : base(parent, locator)
        {
            this.parent = parent;
            this.locator = locator;

            if (locator.LocatorType != By.XPath && locator.LocatorType != By.CssSelector)
                throw new ArgumentException("SpecDrill: For ListElement<> only Css or XPath locators are accepted!");
        }

        public T this[int index]
        {
            get
            {
                if (index < 1)
                    throw new IndexOutOfRangeException("SpecDrill: ListElement<T> index is 1-based!");
                if (index > Count)
                    throw new IndexOutOfRangeException("SpecDrill: ListElement<T>");

                return WebElement.CreateControl<T>(parent, locator.CopyWithIndex(index));
            }
        }

        public T? GetElementByIndex(int index)
        {
            return this[index];
        }

        //public int Count { get {  this.FindElements(this.locator)?.Count ?? 0; } }

        public new bool IsReadOnly => true;

        public T GetElementByText(string regex)
        {
            var match = this.FirstOrDefault(item => Regex.IsMatch(item.Text, regex));

            if (match == default(T))
                throw new Exception($"SpecDrill: No element matching '{regex}' was found!");

            return match;
        }

        //public U GetChildNodeByText<U>(T node, IElementLocator childrenLocator, string regex)
        //    where U : WebControl, IElement
        //{
        //    var children = WebElement.CreateList<U>(node, childrenLocator);
        //    return children.GetElementByText(regex);
        //}

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Enumerator;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.Enumerator;
        }

        private IEnumerator<T> Enumerator
        {
            get {
                if (this.Count > 0)
                {
                    for (int i = 1; i <= this.Count; i++)
                    {
                        var currentElement = this[i];
                        yield return currentElement;
                    }
                }
            }
        }
    }
}

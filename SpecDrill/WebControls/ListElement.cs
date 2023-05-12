using SpecDrill.Secondary.Ports.AutomationFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpecDrill
{
    public interface IListElement<out T> : IReadOnlyList<T>, IEnumerable, IEnumerable<T>
       where T : class, IElement
    {
        T GetElementByText(string regex);
        T GetElementByIndex(int index);
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

                return ElementFactory.CreateControl<T>(parent, locator.CopyWithIndex(index)) ;
            }
        }

        public T GetElementByIndex(int index)
        {
            return this[index];
        }

        //public int Count { get {  this.FindElements(this.locator)?.Count ?? 0; } }

        public new bool IsReadOnly => true;

        public T GetElementByText(string regex)
        {
            var elements = this.ToArray();
            var match = elements.FirstOrDefault(
                item => 
                    Regex.IsMatch(item.Text, regex)
                    );

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
            get
            {
                var count = this.Count;
                if (count > 0)
                {
                    for (int i = 1; i <= count; i++)
                    {
                        yield return this[i];
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecDrill.Secondary.Ports.AutomationFramework
{
    public interface ISearchable
    {
        /// <summary>
        /// Finds elements matching provided locator
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        ReadOnlyCollection<ISearchable> FindElements(IElementLocator locator);
        public bool IsShadowRoot();
        public ISearchable GetShadowRoot();
        public object NativeElement { get; }
    }
}

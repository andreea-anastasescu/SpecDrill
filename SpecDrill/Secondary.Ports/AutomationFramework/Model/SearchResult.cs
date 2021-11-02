using System.Linq;

namespace SpecDrill.Secondary.Ports.AutomationFramework.Model
{
    public class SearchResult
    {
        public class NullElementLocator : IElementLocator
        {
            public By LocatorType => throw new System.NotImplementedException(nameof(NullElementLocator));

            public string LocatorValue => throw new System.NotImplementedException(nameof(NullElementLocator));

            public int? Index => throw new System.NotImplementedException(nameof(NullElementLocator));

            public bool IsShadowRoot => throw new System.NotImplementedException(nameof(NullElementLocator));

            public IElementLocator Copy()
                => throw new System.NotImplementedException(nameof(NullElementLocator));

            public IElementLocator CopyWithIndex(int index, bool isShadowRoot = false)
                => throw new System.NotImplementedException(nameof(NullElementLocator));
            public override string ToString()
                => "NULL LOCATOR";
        }
        private SearchResult(IElementLocator locator, SearchResult? container, params object[] elements)
            => (Locator, Container, Elements) = (locator, container, elements);
        public object[] Elements { get; private set; } = new object[0];
        public IElementLocator Locator { get; private set; }
        public SearchResult? Container { get; private set; }
        public int Count => Elements.Length;
        public static SearchResult Create(IElementLocator locator,SearchResult? container, params object[] elements)
            => new SearchResult(locator, container, elements);

        public static SearchResult Empty => Create(new NullElementLocator(), default);

        public bool HasResult => Elements.Any();
    }
}

using System.Linq;

namespace SpecDrill.Secondary.Ports.AutomationFramework.Model
{
    public class SearchResult
    {
        private SearchResult(IElementLocator? locator, SearchResult? container, params object[] elements)
            => (Locator, Container, Elements) = (locator, container, elements);
        public object[] Elements { get; private set; } = new object[0];
        public IElementLocator? Locator { get; private set; }
        public SearchResult? Container { get; private set; }
        public int Count => Elements.Length;
        public static SearchResult Create(IElementLocator? locator,SearchResult? container, params object[] elements)
            => new SearchResult(locator, container, elements);

        public static SearchResult Empty => Create(default, default);

        public bool HasResult => Elements.Any();
    }
}

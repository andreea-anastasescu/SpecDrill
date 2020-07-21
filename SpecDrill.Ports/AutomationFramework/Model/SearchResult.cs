using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecDrill.SecondaryPorts.AutomationFramework.Model
{
    public class SearchResult
    {
        private SearchResult(object? nativeElement, int count) { this.NativeElement = nativeElement; this.Count = count; }
        public object? NativeElement { get; }
        public int Count { get; }
        public static SearchResult Create(object? nativeElement, int count)
        {
            return new SearchResult(nativeElement, count);
        }

        public static SearchResult Empty => Create(null, 0);

        public bool HasResult => NativeElement != null;
    }
}

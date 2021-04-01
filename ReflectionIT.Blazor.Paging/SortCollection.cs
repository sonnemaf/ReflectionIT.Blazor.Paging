using System.Collections.ObjectModel;

namespace ReflectionIT.Blazor.Paging {
    public class SortCollection : KeyedCollection<string, Sort> {
        protected override string GetKeyForItem(Sort item) => item.Key;
    }
}

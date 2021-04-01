using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReflectionIT.Blazor.Paging {
    public interface IPagingClient<T> : IEnumerable<T>, IPagingList, ISortablePagingList, IDisposable where T : class {

        event EventHandler CollectionChanged;

        Func<IPagingClient<T>, string>? Filter { get; set; }

        string GetQueryFilter(string key, string defaultValue);
        void SetQueryFilter(string key, string value);
        public bool IsExecuted { get; }
        public string QueryTemplate { get; set; }
        public int PageSize { get; set; }

        Task ExecuteAsync(string? sortExpression = default, int? pageIndex = default);
    }
}
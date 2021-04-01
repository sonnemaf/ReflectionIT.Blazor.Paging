using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace ReflectionIT.Blazor.Paging {

    public class PagingClient<TModel, TResponse> : IPagingClient<TModel>, IDisposable where TModel : class where TResponse : IPagedResponse<TModel> {

        private bool _isDisposed;
        private readonly HttpClient _client;
        private TModel[] _data = Array.Empty<TModel>();

        protected Dictionary<string, string> QueryStrings { get; } = new();
        public Func<IPagingClient<TModel>, string>? Filter { get; set; }

        public SortCollection Sorts { get; } = new SortCollection();
        public int TotalRecordCount { get; private set; }
        public bool IsExecuted { get; private set; }
        public int PageSize { get; set; } = 10;
        public NavigationManager NavMan { get; }
        public string QueryTemplate { get; set; } = string.Empty;
        public int PageIndex { get; set; } = 1;
        public string? SortExpression { get; set; }

        public event EventHandler? CollectionChanged;

        protected virtual void OnCollectionChanged() {
            CollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public PagingClient(HttpClient client, NavigationManager navMan) {
            this._client = client;
            this.NavMan = navMan;
            this.NavMan.LocationChanged += NavMan_LocationChanged;
            InitFromUri(NavMan.Uri);
        }

        public string GetQueryFilter(string key, string defaultValue) => QueryStrings.GetValueOrDefault(key, defaultValue);

        public void SetQueryFilter(string key, string value) {
            if (string.IsNullOrEmpty(value)) {
                if (QueryStrings.ContainsKey(key)) {
                    QueryStrings.Remove(key);
                }
            } else {
                QueryStrings[key] = value;
            }
        }

        private void InitFromUri(string uri) {
            var dict = QueryHelpers.ParseQuery(NavMan.ToAbsoluteUri(uri).Query);
            SortExpression = dict.GetOrDefault<string>("sort", null);
            PageIndex = dict.GetOrDefault("pageIndex", 1);
            foreach (var item in dict) {
                if (item.Key != "sort" && item.Key != "pageIndex") {
                    QueryStrings[item.Key] = item.Value;
                }
            }
        }

        public async Task ExecuteAsync(string? sortExpression = default, int? pageIndex = default) {
            if (pageIndex.HasValue) {
                PageIndex = pageIndex.Value;
            }
            if (sortExpression is not null) {
                SortExpression = sortExpression;
            }
            if (SortExpression is null) {
                SortExpression = this.Sorts[0].Key;
            }

            var url = CreateUrl().ToString();

            var result = await _client.GetFromJsonAsync<TResponse>(url);

            if (result is not null) {
                _data = result.Value ?? Array.Empty<TModel>();
                TotalRecordCount = result.Count;
            } else {
                _data = Array.Empty<TModel>();
                TotalRecordCount = 0;
            }

            IsExecuted = true;
            OnCollectionChanged();
        }

        protected virtual StringBuilder CreateUrl() {
            var sb = new StringBuilder(QueryTemplate);
            sb.Replace("{skip}", ((PageIndex - 1) * PageSize).ToString());
            sb.Replace("{top}", PageSize.ToString());
            sb.Replace("{PageIndex}", PageIndex.ToString());
            sb.Replace("{PageSize}", PageSize.ToString());
            sb.Replace("{filter}", Filter is not null ? Filter(this) : string.Empty);

            var orderby = SortExpression;
            orderby = orderby![0] == '-' ? this.Sorts[orderby[1..]].SortExpression2 : this.Sorts[orderby].SortExpression;
            sb.Replace("{orderby}", orderby);

            return sb;
        }

        public int PageCount => (int)Math.Ceiling(TotalRecordCount / (double)PageSize);

        public int Count => IsExecuted ? _data.Length : -1;

        public TModel this[int index] => IsExecuted ? _data[index] : throw new InvalidOperationException("ODataPagingCollection must be executed first");

        public IEnumerator<TModel> GetEnumerator() => _data == null ? Enumerable.Empty<TModel>().GetEnumerator() : ((IEnumerable<TModel>)this._data).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this._data.GetEnumerator();

        public string GetHref() => GetHref(this.PageIndex);

        public string GetHref(int pageIndex) {
            var uri = NavMan.ToAbsoluteUri(NavMan.Uri).GetLeftPart(UriPartial.Path);
            uri = QueryHelpers.AddQueryString(uri, new Dictionary<string, string>(QueryStrings) {
                ["sort"] = this.SortExpression ?? string.Empty,
                ["pageIndex"] = pageIndex.ToString(),
            });
            return uri;
        }

        public string GetHref(string sortExpression) {
            if (sortExpression == this.SortExpression) {
                // Toggle Ascending/Decending
                sortExpression = sortExpression[0] == '-' ? sortExpression[1..] : "-" + sortExpression;
            }

            var uri = NavMan.ToAbsoluteUri(NavMan.Uri).GetLeftPart(UriPartial.Path);
            uri = QueryHelpers.AddQueryString(uri, new Dictionary<string, string>(QueryStrings) {
                ["sort"] = sortExpression,
                ["pageIndex"] = "1",
            });
            return uri;
        }


        public void Dispose() {
            _isDisposed = true;
            this.NavMan.LocationChanged -= NavMan_LocationChanged;
            GC.SuppressFinalize(this);
        }

        private async void NavMan_LocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e) {
            if (!_isDisposed) {
                InitFromUri(e.Location);
                await this.ExecuteAsync();
            }
        }
    }

}

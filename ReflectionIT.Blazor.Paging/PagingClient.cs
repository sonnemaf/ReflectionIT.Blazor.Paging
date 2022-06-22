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

        private bool _isExecuting;

        protected bool IsDisposed { get; set; }
        protected HttpClient Client { get; }
        protected TModel[] Data { get; set; } = Array.Empty<TModel>();
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
            this.Client = client;
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
            try {
                if (!_isExecuting) {
                    _isExecuting = true;
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

                    var result = await Client.GetFromJsonAsync<TResponse>(url);

                    if (result is not null) {
                        Data = result.Value ?? Array.Empty<TModel>();
                        TotalRecordCount = result.Count;
                    } else {
                        Data = Array.Empty<TModel>();
                        TotalRecordCount = 0;
                    }

                    IsExecuted = true;
                    OnCollectionChanged();
                }
            } finally {
                _isExecuting = false;
            }
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

        public int Count => IsExecuted ? Data.Length : -1;

        public TModel this[int index] => IsExecuted ? Data[index] : throw new InvalidOperationException("ODataPagingCollection must be executed first");

        public IEnumerator<TModel> GetEnumerator() => Data == null ? Enumerable.Empty<TModel>().GetEnumerator() : ((IEnumerable<TModel>)this.Data).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.Data.GetEnumerator();

        public string GetHref() => GetHref(this.PageIndex);

        public string GetHref(int pageIndex) {
            var uri = NavMan.ToAbsoluteUri(NavMan.Uri).GetLeftPart(UriPartial.Path);

            var queryString = new Dictionary<string, string>(QueryStrings);
            if (this.SortExpression != this.Sorts[0].SortExpression) {
                queryString["sort"] = this.SortExpression ?? string.Empty;
            };
            if (pageIndex > 1) {
                queryString["pageIndex"] = pageIndex.ToString();
            }
            uri = QueryHelpers.AddQueryString(uri, queryString);
            return uri;
        }

        public string GetHref(string sortExpression) {
            if (sortExpression == this.SortExpression) {
                // Toggle Ascending/Decending
                sortExpression = sortExpression[0] == '-' ? sortExpression[1..] : "-" + sortExpression;
            }
            var uri = NavMan.ToAbsoluteUri(NavMan.Uri).GetLeftPart(UriPartial.Path);
            var queryString = new Dictionary<string, string>(QueryStrings);
            
            if (sortExpression != this.Sorts[0].SortExpression) {
                queryString["sort"] = sortExpression ?? string.Empty;
            };
            
            uri = QueryHelpers.AddQueryString(uri, queryString);
            return uri;
        }


        public void Dispose() {
            IsDisposed = true;
            this.NavMan.LocationChanged -= NavMan_LocationChanged;
            GC.SuppressFinalize(this);
        }

        private async void NavMan_LocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e) {
            if (!IsDisposed) {
                InitFromUri(e.Location);
                await this.ExecuteAsync();
            }
        }

        public void ReExecute() {
            this.NavMan.NavigateTo(GetHref(1), false);
        }
    }

}

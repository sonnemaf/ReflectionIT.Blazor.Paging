using System.Net.Http;
using Microsoft.AspNetCore.Components;
using System.Text;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ReflectionIT.Blazor.Paging {
    public class ODataPagingClient<TModel> : PagingClient<TModel, ODataResult<TModel>>, IODataPagingClient<TModel> where TModel : class {

        [AllowNull]
        public string Path { get; set; }
        public string? Select { get; set; }
        public string? Expand { get; set; }

        public ODataPagingClient(HttpClient client, NavigationManager navMan) : base(client, navMan) {
            this.QueryTemplate = "{path}?$count=true&$top={top}&$skip={skip}&$orderby={orderby}{select}{expand}{filter}";
        }

        protected override StringBuilder CreateUrl() {
            if (string.IsNullOrEmpty(Path)) {
                throw new InvalidOperationException("Path must be set");
            }
            var sb = base.CreateUrl();
            sb.Replace("{path}", Path);
            sb.Replace("{select}", !string.IsNullOrEmpty(Select) ? $"&$select={Select}" : string.Empty);
            sb.Replace("{expand}", !string.IsNullOrEmpty(Expand) ? $"&$expand={Expand}" : string.Empty);
            return sb;
        }
    }

}

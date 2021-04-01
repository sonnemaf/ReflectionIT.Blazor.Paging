using System.Text.Json.Serialization;

namespace ReflectionIT.Blazor.Paging {

    public class ODataResult<T> : IPagedResponse<T> {

        [JsonPropertyName("value")]
        public T[]? Value { get; set; }

        [JsonPropertyName("@odata.count")]
        public int Count { get; set; }

        [JsonPropertyName("@odata.context")]
        public string? ODataContext { get; set; }
    }

}

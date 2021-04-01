namespace ReflectionIT.Blazor.Paging {

    public interface IODataPagingClient<T> : IPagingClient<T> where T : class {
        string Path { get; set; }
        string? Expand { get; set; }
        string? Select { get; set; }
    }
}
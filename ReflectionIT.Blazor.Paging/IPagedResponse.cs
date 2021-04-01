namespace ReflectionIT.Blazor.Paging {
    public interface IPagedResponse<T> {
        int Count { get; }
        T[]? Value { get; }
    }
}
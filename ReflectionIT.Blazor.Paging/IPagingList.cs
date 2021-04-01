namespace ReflectionIT.Blazor.Paging {

#nullable enable

    public interface IPagingList {
        int PageCount { get; }
        int PageIndex { get; }
        int TotalRecordCount { get; }
        string GetHref(int pageIndex);
    }
}

namespace ReflectionIT.Blazor.Paging {
    public interface ISortablePagingList {
        int PageIndex { get; }
        string? SortExpression { get; set; }
        SortCollection Sorts { get; }

        string GetHref(string sortExpression);
    }
}

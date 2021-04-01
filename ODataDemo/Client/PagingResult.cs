using ReflectionIT.Blazor.Paging;

namespace ODataDemo.Client {

    public class PagingResult<T> : IPagedResponse<T> {

        public T[] Data { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        public string Sort { get; set; }

        int IPagedResponse<T>.Count => RecordCount;

        T[] IPagedResponse<T>.Value => Data;
    }

}

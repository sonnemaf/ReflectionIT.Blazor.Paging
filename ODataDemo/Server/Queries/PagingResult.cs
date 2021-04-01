namespace ODataDemo.Server.Queries {

    public class PagingResult<T> {

        public T[] Data { get; }
        public int PageIndex { get; }
        public int PageSize { get; }
        public int RecordCount { get; }
        public string Sort { get; }


        public PagingResult(T[] data, int pageIndex, int pageSize, int recordCount, string sort) {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            RecordCount = recordCount;
            Sort = sort;
        }

    }

}

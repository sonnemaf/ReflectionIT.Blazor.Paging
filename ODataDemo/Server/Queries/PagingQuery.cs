using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ODataDemo.Server.Queries {

    public class PagingQuery<TEntity> {

        public const int DefaultPageSize = 10;

        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public string Sort { get; set; }

        public virtual async Task<PagingResult<TEntity>> ExecuteAsync(IQueryable<TEntity> query, string defaultSort) {
            Sort ??= defaultSort;
            var filter = Filter(query);
            var count = await filter.CountAsync();

            var qry = TakeAndSkip(OrderBy(filter));
            var data = await qry.ToArrayAsync();

            return new PagingResult<TEntity>(data, PageIndex ?? 1, PageSize ?? DefaultPageSize, count, Sort);
        }

        protected virtual IQueryable<TEntity> Filter(IQueryable<TEntity> query) => query;

        // Install-Package System.Linq.Dynamic.Core 
        protected virtual IQueryable<TEntity> OrderBy(IQueryable<TEntity> query) => query.OrderBy(Sort);

        protected virtual IQueryable<TEntity> TakeAndSkip(IQueryable<TEntity> query) {
            return query.Skip(((PageIndex ?? 1) - 1) * (PageSize ?? DefaultPageSize))
                        .Take(PageSize ?? DefaultPageSize);
        }

    }
}

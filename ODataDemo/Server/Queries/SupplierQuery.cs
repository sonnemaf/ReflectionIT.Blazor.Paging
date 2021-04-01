using Microsoft.EntityFrameworkCore;
using ODataDemo.Shared;
using System.Linq;

namespace ODataDemo.Server.Queries {
    public class ProductsQuery : PagingQuery<Product> {

        public string ProductNameFilter { get; set; }

        protected override IQueryable<Product> Filter(IQueryable<Product> query) {
            if (!string.IsNullOrEmpty(ProductNameFilter)) {
                query = query.Where(s => EF.Functions.Like(s.ProductName, ProductNameFilter + "%"));
            }
            return query;
        }
    }
}

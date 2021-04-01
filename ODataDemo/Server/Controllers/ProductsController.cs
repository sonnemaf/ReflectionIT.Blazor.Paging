using System.Linq;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ODataDemo.Server.Data;
using ODataDemo.Shared;

namespace ODataDemo.Server.Controllers {

    public class ProductsController : ODataController {

        private readonly NorthwindContext _context;

        public ProductsController(NorthwindContext context) => _context = context;

        // GET /odata/Products
        [EnableQuery]
        public IQueryable<Product> Get() {
            return _context.Products;
        }

        // GET /odata/Products(1)
        public Product Get(int key) {
            return _context.Products.Find(key);
        }
    }
}

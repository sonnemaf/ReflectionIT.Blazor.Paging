using System.Linq;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ODataDemo.Server.Data;
using ODataDemo.Shared;

namespace ODataDemo.Server.Controllers {
    public class CategoriesController : ODataController {

        private readonly NorthwindContext _context;

        public CategoriesController(NorthwindContext context) {
            _context = context;
}

        // GET /odata/Categories
        [EnableQuery]
        public IQueryable<Category> Get() {
            return _context.Categories;
        }

        // GET /odata/Categories(1)
        public Category Get(int key) {
            return _context.Categories.Find(key);
        }
    }
}

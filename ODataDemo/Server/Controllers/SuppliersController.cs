using System.Linq;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ODataDemo.Server.Data;
using ODataDemo.Shared;

namespace ODataDemo.Server.Controllers {
    public class SuppliersController : ODataController {

        private readonly NorthwindContext _context;

        public SuppliersController(NorthwindContext context) {
            _context = context;
}

        // GET /odata/Suppliers
        [EnableQuery]
        public IQueryable<Supplier> Get() {
            return _context.Suppliers;
        }

        // GET /odata/Suppliers(1)
        public Supplier Get(int key) {
            return _context.Suppliers.Find(key);
        }
    }
}

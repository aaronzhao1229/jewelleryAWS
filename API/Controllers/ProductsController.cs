using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        // we are going to use dependency injection to get our store context inside here so that wwe've got access to the products table in our database
        // In order to use dependency injection, we create a private field inside our class and assign that private fields to the context that we're adding in our constructor here
        private readonly StoreContext _context;
        public ProductsController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        // we are going to return a list of products from this method, but what we typically do inside an API controller is specify the type of result we're returning, so we can return the type of ActionResult
        public async Task<ActionResult<List<Product>>> GetProducts() // use async then need Task to wrap the ActionResult
        {
            return await _context.Products.ToListAsync();
        }

        [HttpGet("{id}")] // api/products/3
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) return NotFound();

            return product;
        }


    }
}
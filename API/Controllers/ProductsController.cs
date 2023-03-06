using System.Text.Json;
using API.Data;
using API.Entities;
using API.Extensions;
using API.RequestHelpers;
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
        public async Task<ActionResult<PagedList<Product>>> GetProducts([FromQuery]ProductParams productParams) // use async then need Task to wrap the ActionResult
        // if the parameters are strings, api controller will presume these strings are going to be passed as query string parameters. If the parameters are objects, it will presume the objects are from the query body. So if we want to use objects as parameter and get it from query strings, we need to add [FromQuery]
        {
            // return await _context.Products.ToListAsync();
            var query = _context.Products
                .Sort(productParams.OrderBy)
                .Search(productParams.SearchTerm)
                .Filter(productParams.Categories)
                .AsQueryable();
            
            var products = await PagedList<Product>.ToPagedList(query, productParams.PageNumber, productParams.PageSize);

            // what we'll do with these product parameters is we're going to return them in our response headers and get access to our pagination from our response headers as well. 

            Response.AddPaginationHeader(products.MetaData);
            
            return products;
        }

        [HttpGet("{id}")] // api/products/3
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) return NotFound();

            return product;
        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters() //if we're using IActionResult, we get access to all of the Http Responses, such as NotFound, return OK,  we just don't get type safety with our response. We'll create an anonymous object and return that from this result.
        {
            var categories = await _context.Products.Select(p => p.Category).Distinct().ToListAsync();

            return Ok(new {categories});
        }


    }
}
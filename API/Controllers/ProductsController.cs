using System.Text.Json;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.RequestHelpers;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        // we are going to use dependency injection to get our store context inside here so that wwe've got access to the products table in our database
        // In order to use dependency injection, we create a private field inside our class and assign that private fields to the context that we're adding in our constructor here
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly ImageService _imageService;
        public ProductsController(StoreContext context, IMapper mapper, ImageService imageService)
        {
            _imageService = imageService;
            _mapper = mapper;
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

        [HttpGet("{id}", Name = "GetProduct")] // api/products/3
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

        [Authorize(Roles = "Admin")] // only admin can create a product
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromForm]CreateProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);

            if (productDto.File != null)
            {
                var imageResult = await _imageService.AddImageAsync(productDto.File);

                if (imageResult.Error != null) return BadRequest(new ProblemDetails{Title = imageResult.Error.Message});

                product.PictureUrl = imageResult.SecureUrl.ToString();
                product.PublicId = imageResult.PublicId;
            }

            _context.Products.Add(product);

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return CreatedAtRoute("GetProduct", new {Id = product.Id}, product);
            // we use CreateAtRoute because we're creating a new resource on the server and we will use the overload that takes the route name, route value here will be the product.id because we want to be able to get an individual product. Then return the product from this method.
            return BadRequest(new ProblemDetails {Title = "Problem creating new product"});
            // because we are not going to take up the picture URL as it is, we will need a DTO to receive the info from the client.
        }

        [Authorize(Roles = "Admin")] // only admin can create a product
        [HttpPut]
        public async Task<ActionResult<Product>> UpdateProduct([FromForm]UpdateProductDto productDto)
        {
            var product = await _context.Products.FindAsync(productDto.Id);

            if (product == null) return NotFound();

            _mapper.Map(productDto, product);

            if (productDto.File != null)
            {
                var imageResult = await _imageService.AddImageAsync(productDto.File);
                if (imageResult.Error != null) return BadRequest(new ProblemDetails{Title = imageResult.Error.Message});

                // we're not going to delete te files from the file system, but we will remove the pic from Cloudinary if we have a cloudinary image
                if (!string.IsNullOrEmpty(product.PublicId)) await _imageService.DeleteImageAsync(product.PublicId);

                product.PictureUrl = imageResult.SecureUrl.ToString();
                product.PublicId = imageResult.PublicId;
            }

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok(product);
            // NoContent() is a technical correct response and it will give us a 204 that says the resource has been updated on a database
            return BadRequest(new ProblemDetails {Title = "Problem updating product"});
        }

        [Authorize(Roles = "Admin")] // only admin can create a product
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) return NotFound();

            if (!string.IsNullOrEmpty(product.PublicId)) await _imageService.DeleteImageAsync(product.PublicId);

            _context.Products.Remove(product);

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok();
           
            return BadRequest(new ProblemDetails {Title = "Problem deleting product"});

        }
  }
}
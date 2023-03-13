using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  public class BasketController : BaseApiController
    {
        private readonly StoreContext _context;
        public BasketController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet(Name =  "GetBasket")]  // add a route name for the CreateAt function in AddItemToBasket
        public async Task<ActionResult<BasketDto>> GetBasket()
        {

            var basket = await RetrieveBasket(GetBuyerId());

            if (basket == null) return NotFound();

            return basket.MapBasketToDto();
        }

   

    [HttpPost] // api/basket?productId=3&quantity=2
        // product Id and quantity is from the query string
        public async Task<ActionResult<BasketDto>> AddItemToBasket(int productId, int quantity)
        {
            // get basket
             var basket = await RetrieveBasket(GetBuyerId());
            // create basket if it does not exist
            if (basket == null) basket = CreateBasket();
            // get product
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return BadRequest(new ProblemDetails{Title = "Product Not Found"});
            // add item
            basket.AddItem(product, quantity);
            // save changes
            var result = await _context.SaveChangesAsync() > 0; // SaveChangesAsync returns an integer of the number of changes that have been made in our database. We want to track it greater than 0, which means we have saved changes to the database. If it is not greater than 0, then we will return bad request. 

            if (result) return CreatedAtRoute("GetBasket", basket.MapBasketToDto()); // return the basket with added items

            return BadRequest(new ProblemDetails{Title = "Problem saving item to basket"});
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveBasketItem(int productId, int quantity)
        {
            // get basket
            var basket = await RetrieveBasket(GetBuyerId());
            if (basket == null) return NotFound();
            // remove item or reduct quantity
            basket.RemoveItem(productId, quantity);
            // save changes
            var result = await _context.SaveChangesAsync() > 0;
            if (result) return Ok();
            return BadRequest(new ProblemDetails{Title = "Problem removing item from basket"});
        }

         private async Task<Basket> RetrieveBasket(string buyerId)
        {
          // We pass a buyerId as a parameter. If this value is empty, then we're going to remove the cookie from the response.
          if (string.IsNullOrEmpty(buyerId))
          {
              Response.Cookies.Delete("buyerId");
              return null;
          }
          // if there is a buyerId, that's going to be the usename or the cookie, then we're going to return the baskets associated with that buyerId         

          // we are going to use a cookie. When a user creates a basket on our server, we're going to return them a buyer id, which is going to be sent back to him as a cookie and cookies are stored in a user's browser in storage. So persistence storage. For every request and response, we used a cookie and it goes backwards and forwards between the client and the server. So we'll have access to a cookie
          return await _context.Baskets
              .Include(i => i.Items)
              .ThenInclude(p => p.Product)
              .FirstOrDefaultAsync(x => x.BuyerId == buyerId);

          //.FirstOrDefaultAsync(x => x.BuyerId == Request.Cookies["buyerId"]);
          // when we return FirstOrDefaultAsync, this will not include the basket items unless we explicitly tell it to in this command, so we need to add include() method
        }

        private string GetBuyerId()
        // we are going to check if there is username or a buyerId in the cookie. 
        {
            return User.Identity?.Name ?? Request.Cookies["buyerId"];
        }

        private Basket CreateBasket()
        {
            var buyerId = User.Identity?.Name;
            if (string.IsNullOrEmpty(buyerId))
            {
                buyerId = Guid.NewGuid().ToString(); // generate a buyerId
                var cookieOptions = new CookieOptions{
                IsEssential = true, Expires = DateTime.Now.AddDays(30)
                }; // Our website will not function correctly without this particular cookie. This cookie is essential to the operation of our application. Don't add the HTTP only flag onto the cookie. The HTTP only means it's only going to be sent and received over network requests over HTTP requests. And that makes it impossible for us to retrieve it from JS or TS. There will be an occasion where we do need to retrieve the basket Id or the buyerId inside our client side code. 
                Response.Cookies.Append("buyerId", buyerId, cookieOptions);
            }
            
            
            var basket = new Basket { BuyerId = buyerId };
            _context.Baskets.Add(basket); // EF will start tracking this entity we added
            return basket;
        }
    }
}
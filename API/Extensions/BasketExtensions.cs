using API.DTOs;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
  public static class BasketExtensions
    {

        public static BasketDto MapBasketToDto(this Basket basket) // this is the class we are extending and we'll call it baskets
        // if we're calling this method and we're passing in a basket as our parametere when we're extending something, then our extension method is expecting to receive a basket at this point. And when it tries to return a new basket without having a basket, then it's going to give us no reference error or objects not set to an instance of object. So we need to use ? when we pass the basket to here (in the AccountController)
          {
            return new BasketDto
            {
              Id = basket.Id,
              BuyerId = basket.BuyerId,
              Items = basket.Items.Select(item => new BasketItemDto
              {
                ProductId = item.ProductId,
                Name = item.Product.Name,
                Price = item.Product.Price,
                PictureUrl = item.Product.PictureUrl,
                Category = item.Product.Category,
                Quantity = item.Quantity
              }).ToList()
            };
          }
        
        public static IQueryable<Basket> RetrieveBasketWithItems(this IQueryable<Basket> query, string buyerId)
        {
            return query.Include(i => i.Items).ThenInclude(p => p.Product).Where(b => b.BuyerId == buyerId);
        }
    }
}
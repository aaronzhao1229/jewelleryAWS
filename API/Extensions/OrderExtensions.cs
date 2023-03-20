using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class OrderExtensions
    {
        public static IQueryable<OrderDto> ProjectOrdertoOrderDto(this IQueryable<Order> query)
        {
           return query
              .Select(order => new OrderDto
              {
                  Id = order.Id,
                  BuyerId = order.BuyerId,
                  OrderDate = order.OrderDate,
                  ShippingAddress = order.ShippingAddress,
                  DeliveryFee = order.DeliveryFee,
                  Subtotal = order.Subtotal,
                  OrderStatus = order.OrderStatus.ToString(),
                  Total = order.GetTotal(),
                  OrderItems = order.OrderItems.Select(item => new OrderItemDto
                  {
                      ProductId = item.ItemOrdered.ProductId,
                      Name = item.ItemOrdered.Name,
                      PictureUrl = item.ItemOrdered.PictureUrl,
                      Price = item.Price,
                      Quantity = item.Quantity,

                  }).ToList()
              }).AsNoTracking(); // we need to use this as we don't need  framework to track what we get. Otherwise there will be error (A tracking query is attempting to project an owned entity without a corresponding owner in its result, but owned entities cannot be tracked without their owner. Either include the owner entity in the result or make the query non-tracking using 'AsNoTracking')
        }
    }
}
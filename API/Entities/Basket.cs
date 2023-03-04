using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Basket
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public List<BasketItem> Items { get; set; } = new List<BasketItem>(); // create an intial list for the basket

        public void AddItem(Product product, int quantity)
        {   // check if the product already in basket. If not, add to basket
            if (Items.All(item => item.ProductId != product.Id)) 
            {
                Items.Add(new BasketItem{Product = product, Quantity = quantity});
            }

            var existingItem = Items.FirstOrDefault(item => item.ProductId == product.Id);
            if (existingItem != null) existingItem.Quantity += quantity;
        }

        public void RemoveItem(int productId, int quantity)
        {
           var item = Items.FirstOrDefault(item => item.ProductId == productId);
           if (item == null) return;
           item.Quantity -= quantity;
           if (item.Quantity == 0) Items.Remove(item);
        }
    }
}
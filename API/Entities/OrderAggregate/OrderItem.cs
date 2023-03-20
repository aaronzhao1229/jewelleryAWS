using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities.OrderAggregate
{
    public class OrderItem
    {
        public int Id { get; set; }
        public ProductItemOrdered ItemOrdered { get; set; }
        // Now a productItemOrdered was one of those owed properties. So in our order item table, we can see the ItemOrderd as well in the OrderItem table along with these other properties. That's what it means when we own an entity in this way.
        public long Price { get; set; }
        public int Quantity { get; set; }
    }
}
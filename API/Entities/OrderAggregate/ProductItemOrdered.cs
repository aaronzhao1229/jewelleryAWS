using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.OrderAggregate
{
    [Owned]
    public class ProductItemOrdered
    // this is going to contain a snapshot of the item as it was when it was orderd. So if somebody comes along later on and changes a particular property that will always have the historic property as it was when the item was ordered inside this table, because the user is going to be able to get a list of their orders. If you change the name of something a bit later on or the image, then it might be a bit confusing for the user when they look at their previous orders.
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }

    }
}
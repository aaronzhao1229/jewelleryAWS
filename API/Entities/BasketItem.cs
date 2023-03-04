using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
  [Table("BasketItems")] // use data annotation to define table name
  public class BasketItem
  {
      public int Id { get; set; }
      public int Quantity { get; set; }

      // we need a relation beween our baskets item and the products here. The way we do that in entity framework is we give our baskets item what's referred to is navigation properties

      // fully defined relationship, this means the basket item cannot exist without the Product or the basket
      public int ProductId { get; set; }
      public Product Product { get; set; }

      public int BasketId { get; set; }
      public Basket Basket { get; set; }


  }
}
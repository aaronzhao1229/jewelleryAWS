using Microsoft.EntityFrameworkCore;

namespace API.Entities.OrderAggregate
{
    [Owned]
    // there is no id property for this case, rather than this being a related property that lives in another table or related entity that live in another table, we are going to make it our order owns this particular entity, which means we do not need to give it an id property because the properties inside this entity are going to in the owner table. 
    public class ShippingAddress : Address
      {
          
      }
}
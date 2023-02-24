using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  public class StoreContext : DbContext
  {
    public StoreContext(DbContextOptions options) : base(options) // because we're deriving from a class, then we/ve got a base class, which is the DB context we're deriving from. We are passing the options up to the base class (DbContext) when we do so.
    // but when we use our storeContext, we need to pass it some options, such as the connection string we use to connect to the database. 
    {
    }

    public DbSet<Product> Products { get; set; }  // represent a table for products in our database. 
  }
}
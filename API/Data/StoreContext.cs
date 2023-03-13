using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  public class StoreContext : IdentityDbContext<User> // DbContext
  {
    public StoreContext(DbContextOptions options) : base(options) // because we're deriving from a class, then we/ve got a base class, which is the DB context we're deriving from. We are passing the options up to the base class (DbContext) when we do so.
    // but when we use our storeContext, we need to pass it some options, such as the connection string we use to connect to the database. 
    {
    }

    public DbSet<Product> Products { get; set; }  // represent a table for products in our database. 

    public DbSet<Basket> Baskets { get; set; }

    // We can use OnModelCreating method in the DbContext and we can overrride our storeContext class and do something with it
    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder); //It provides a model builder class as a parameter and then calls into the base OnModelCreating class from the IdentityDBContext and passes it the builder. 

      builder.Entity<IdentityRole>() // get role
          .HasData(
              new IdentityRole{Name = "Member", NormalizedName = "MEMBER"},
              new IdentityRole{Name = "Admin", NormalizedName = "ADMIN"}
          );
      // the idea of this is that when we create a migration, we're going to have some SQL commands to insert data into our tables, and one of our table is going to be for our roles.
    }
  }
}
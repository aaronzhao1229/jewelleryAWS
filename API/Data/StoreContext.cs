using API.Entities;
using API.Entities.OrderAggregate;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  public class StoreContext : IdentityDbContext<User, Role, int> // DbContext. Add int here to specify all of our identity classes are going to use an integer as their ID
  {
    public StoreContext(DbContextOptions options) : base(options) // because we're deriving from a class, then we/ve got a base class, which is the DB context we're deriving from. We are passing the options up to the base class (DbContext) when we do so.
    // but when we use our storeContext, we need to pass it some options, such as the connection string we use to connect to the database. 
    {
    }

    public DbSet<Product> Products { get; set; }  // represent a table for products in our database. 

    public DbSet<Basket> Baskets { get; set; }

    public DbSet<Order> Orders { get; set; }

    // We can use OnModelCreating method in the DbContext and we can overrride our storeContext class and do something with it
    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder); //It provides a model builder class as a parameter and then calls into the base OnModelCreating class from the IdentityDBContext and passes it the builder. 

      builder.Entity<User>()
          .HasOne(a => a.Address) 
          .WithOne() // defining one to one relationship. A user has one address with one user effectively
          .HasForeignKey<UserAddress>(a => a.Id)
          .OnDelete(DeleteBehavior.Cascade);
      // this is an alternative approach to the fully defined relation ship between the user and userAddress. (fluent configuration)

      builder.Entity<Role>() // get role
          .HasData(
              new Role{Id = 1, Name = "Member", NormalizedName = "MEMBER"},
              new Role{Id = 2, Name = "Admin", NormalizedName = "ADMIN"}
          );
      // the idea of this is that when we create a migration, we're going to have some SQL commands to insert data into our tables, and one of our table is going to be for our roles.
    }
  }
}
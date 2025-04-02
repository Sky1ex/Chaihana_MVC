using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebApplication1.Models;

namespace WebApplication1.DataBase
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            //_ = Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

        public DbSet<Product> Products { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Order> Orders { get; set; } 

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<CartElement> CartElements { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<OrderElement> OrderElements { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Adress> Adresses { get; set; }

        public DbSet<AddressElement> AddressElements { get; set; }
    }
}

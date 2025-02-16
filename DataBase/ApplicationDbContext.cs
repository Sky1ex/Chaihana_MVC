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
            /*modelBuilder.Entity<Order>().OwnsMany(o => o.ProductCounts, pc =>
            {
                pc.WithOwner().HasForeignKey("OrderId");
                pc.Property<int>("Id").ValueGeneratedOnAdd();
                pc.HasKey("Id");
            });*/

            modelBuilder.Entity<Order>(order =>
            {
                order.HasKey(o => o.OrderId); // Указываем первичный ключ

                // Настройка вложенной сущности ProductCount
                order.OwnsMany(o => o.ProductCounts, pc =>
                {
                    pc.WithOwner().HasForeignKey("OrderId"); // Внешний ключ для связи с Order
                    pc.Property<int>("Id").ValueGeneratedOnAdd(); // Скрытый идентификатор
                    pc.HasKey("Id"); // Указываем первичный ключ
                });
            });     

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Order> Orders { get; set; } 

        public DbSet<Admin> Admins { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<ProductCount> ProductsCount { get; set; }
    }
}

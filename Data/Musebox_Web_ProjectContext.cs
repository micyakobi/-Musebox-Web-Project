using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Musebox_Web_Project.Models;

namespace Musebox_Web_Project.Data
{
    public class Musebox_Web_ProjectContext : DbContext
    {
        public Musebox_Web_ProjectContext (DbContextOptions<Musebox_Web_ProjectContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //many to many between orders and products

            modelBuilder.Entity<OrderProduct>()
                .HasKey(po => new { po.OrderId, po.ProductId });

            modelBuilder.Entity<OrderProduct>()
                .HasOne(po => po.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(po => po.OrderId);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(po => po.Product)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(po => po.ProductId);

            //many to many between users(Their carts) and products

            modelBuilder.Entity<UserProduct>()
                .HasKey(po => new { po.UserId, po.ProductId });

            modelBuilder.Entity<UserProduct>()
                .HasOne(po => po.User)
                .WithMany(o => o.UserProducts)
                .HasForeignKey(po => po.UserId);

            modelBuilder.Entity<UserProduct>()
                .HasOne(po => po.Product)
                .WithMany(o => o.UserProducts)
                .HasForeignKey(po => po.ProductId);

        }

        public DbSet<Musebox_Web_Project.Models.Product> Products { get; set; }

        public DbSet<Musebox_Web_Project.Models.Brand> Brand { get; set; }

        public DbSet<Musebox_Web_Project.Models.User> Users { get; set; }

        public DbSet<Musebox_Web_Project.Models.Order> Order { get; set; }

        public DbSet<Musebox_Web_Project.Models.Branch> Branch { get; set; }

    }
}

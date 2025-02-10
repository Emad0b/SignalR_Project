using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ChatRoomSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatRoomSystem.Data
{
    public class MyProjectContext : DbContext
    {
        public MyProjectContext(DbContextOptions<MyProjectContext> options)
            : base(options)
        {
        }
        public MyProjectContext()
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory()) // Make sure to include System.IO
                    .AddJsonFile("appsettings.json")
                    .Build();

                string connectionString = configuration.GetConnectionString("MyConnection");

                optionsBuilder.UseSqlServer(connectionString);
            }

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
                .Property(x => x.Id)
                .HasDefaultValueSql("NEWID()");

            
            modelBuilder.Entity<Message>()
                .Property(x => x.Id)
                .HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Message>()
                .Property(x => x.Timestamp)
                .HasDefaultValueSql("GETDATE()"); 



        }
    }
}

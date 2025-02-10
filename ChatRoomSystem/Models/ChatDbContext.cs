using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ChatRoomSystem.Models
{
    public class ChatDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId);

            modelBuilder.Entity<Message>()
                       .Property(m => m.Id)
                       .HasDefaultValueSql("NEWID()");
            
            modelBuilder.Entity<Message>()
                       .Property(m => m.Timestamp)
                       .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<User>()
           .Property(m => m.Id)
           .HasDefaultValueSql("NEWID()");
        }
    }
}

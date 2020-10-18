using Microsoft.EntityFrameworkCore;
using AlbenolOnline.Entities;

namespace AlbenolOnline.Models
{
    public class ModelsContext : DbContext
    {

        public ModelsContext(DbContextOptions<ModelsContext> options)
           : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        }
        public virtual DbSet<User> Users { get; set; }

    }
}

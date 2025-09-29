using Microsoft.EntityFrameworkCore;
using CustomWebAPI.Models;

namespace CustomWebAPI.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<User>().HasData(
          new User { Id = 1, Name = "John" },
          new User { Id = 2, Name = "Jane" }
      );
    }
  }
}
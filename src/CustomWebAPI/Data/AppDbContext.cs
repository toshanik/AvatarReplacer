using Microsoft.EntityFrameworkCore;
using CustomWebAPI.Models;

namespace CustomWebAPI.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Avatar> Avatars { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<User>()
          .HasIndex(u => u.NormalizedName)
          .IsUnique();

      modelBuilder.Entity<Avatar>()
          .HasOne(a => a.User)
          .WithMany(u => u.Avatars)
          .HasForeignKey(a => a.UserId);
    }
  }
}
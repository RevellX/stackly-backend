using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StacklyBackend.Models;
public class AppDbContext : IdentityDbContext<User>
{
    public DbSet<Example> Examples { get; set; }
    public DbSet<User> AppUsers { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Group> Groups { get; set; }

    public string DbPath { get; }

    public AppDbContext()
    {
        // Default path:
        // var folder = Environment.SpecialFolder.LocalApplicationData;
        // var path = Environment.GetFolderPath(folder);
        // DbPath = System.IO.Path.Join(path, "database.db");

        // use the application's base directory (program root) for the DB file
        DbPath = System.IO.Path.Combine(AppContext.BaseDirectory, "database.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Group>()
         .HasOne(g => g.Owner)                // Group has one Owner
         .WithMany(u => u.OwnedGroups)        // User owns many Groups
         .HasForeignKey(g => g.OwnerId)       // FK property
         .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete
    }
}

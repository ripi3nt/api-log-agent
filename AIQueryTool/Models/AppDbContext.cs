using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AIToolbox.Models;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) :  base(options) {}

    public DbSet<TodoItem> TodoItems { get; set; } = null!;
    public DbSet<UserContextHistory> UserContextHistory { get; set; } = null!;
    public DbSet<StoredFile> StoredFiles { get; set; } = null!;
    public DbSet<StoredChunk> StoredChunks { get; set; } = null!;
    public DbSet<Rule> Rules { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("vector");
    }
}
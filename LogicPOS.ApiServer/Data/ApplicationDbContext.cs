using LogicPOS.ApiServer.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Data;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApiUser> ApiUsers => Set<ApiUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApiUser>(entity =>
        {
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Username).HasMaxLength(128);
            entity.Property(user => user.CreatedUtc).IsRequired();
        });
    }
}

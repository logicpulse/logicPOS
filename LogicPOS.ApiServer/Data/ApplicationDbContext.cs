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
    public DbSet<ApiLicense> ApiLicenses => Set<ApiLicense>();
    public DbSet<ApiTerminal> ApiTerminals => Set<ApiTerminal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApiUser>(entity =>
        {
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Username).HasMaxLength(128).IsRequired();
            entity.Property(user => user.PinHash).HasMaxLength(256).IsRequired();
            entity.Property(user => user.PinSalt).HasMaxLength(256).IsRequired();
            entity.Property(user => user.CreatedUtc).IsRequired();
        });

        modelBuilder.Entity<ApiLicense>(entity =>
        {
            entity.HasKey(license => license.Id);
            entity.Property(license => license.Version).HasMaxLength(64).IsRequired();
            entity.Property(license => license.HardwareId).HasMaxLength(64).IsRequired();
            entity.Property(license => license.Name).HasMaxLength(256).IsRequired();
            entity.Property(license => license.Company).HasMaxLength(256).IsRequired();
            entity.Property(license => license.Nif).HasMaxLength(64).IsRequired();
            entity.Property(license => license.Address).HasMaxLength(512).IsRequired();
            entity.Property(license => license.Email).HasMaxLength(256).IsRequired();
            entity.Property(license => license.Phone).HasMaxLength(64).IsRequired();
            entity.Property(license => license.Reseller).HasMaxLength(256).IsRequired();
        });

        modelBuilder.Entity<ApiTerminal>(entity =>
        {
            entity.HasKey(terminal => terminal.Id);
            entity.HasIndex(terminal => terminal.HardwareId).IsUnique();
            entity.Property(terminal => terminal.Code).HasMaxLength(64).IsRequired();
            entity.Property(terminal => terminal.Designation).HasMaxLength(256).IsRequired();
            entity.Property(terminal => terminal.HardwareId).HasMaxLength(64).IsRequired();
            entity.Property(terminal => terminal.CreatedUtc).IsRequired();
            entity.Property(terminal => terminal.UpdatedUtc).IsRequired();
        });
    }
}

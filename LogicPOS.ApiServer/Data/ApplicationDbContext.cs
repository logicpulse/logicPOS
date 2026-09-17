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
    public DbSet<ApiCompanyInfo> ApiCompanyInfos => Set<ApiCompanyInfo>();
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

        modelBuilder.Entity<ApiCompanyInfo>(entity =>
        {
            entity.HasKey(company => company.Id);
            entity.Property(company => company.Name).HasMaxLength(256).IsRequired();
            entity.Property(company => company.BusinessName).HasMaxLength(256).IsRequired();
            entity.Property(company => company.CommercialName).HasMaxLength(256).IsRequired();
            entity.Property(company => company.LogoPng).IsRequired();
            entity.Property(company => company.LogoBmp).IsRequired();
            entity.Property(company => company.Address).HasMaxLength(512).IsRequired();
            entity.Property(company => company.City).HasMaxLength(128).IsRequired();
            entity.Property(company => company.PostalCode).HasMaxLength(64).IsRequired();
            entity.Property(company => company.CountryCode2).HasMaxLength(8).IsRequired();
            entity.Property(company => company.Phone).HasMaxLength(64).IsRequired();
            entity.Property(company => company.MobilePhone).HasMaxLength(64).IsRequired();
            entity.Property(company => company.Email).HasMaxLength(256).IsRequired();
            entity.Property(company => company.Website).HasMaxLength(256).IsRequired();
            entity.Property(company => company.FiscalNumber).HasMaxLength(64).IsRequired();
            entity.Property(company => company.StockCapital).HasMaxLength(64).IsRequired();
            entity.Property(company => company.DocumentFinalLine1).HasMaxLength(256).IsRequired();
            entity.Property(company => company.DocumentFinalLine2).HasMaxLength(256).IsRequired();
            entity.Property(company => company.TaxEntity).HasMaxLength(128).IsRequired();
            entity.Property(company => company.Fax).HasMaxLength(64).IsRequired();
            entity.Property(company => company.TicketFinalLine1).HasMaxLength(256).IsRequired();
            entity.Property(company => company.TicketFinalLine2).HasMaxLength(256).IsRequired();
            entity.Property(company => company.CurrencyCode).HasMaxLength(16).IsRequired();
            entity.Property(company => company.AgtLogo).IsRequired();
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

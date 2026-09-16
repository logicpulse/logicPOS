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
    public DbSet<ApiMovementType> ApiMovementTypes => Set<ApiMovementType>();
    public DbSet<ApiPlace> ApiPlaces => Set<ApiPlace>();
    public DbSet<ApiTable> ApiTables => Set<ApiTable>();
    public DbSet<ApiHoliday> ApiHolidays => Set<ApiHoliday>();
    public DbSet<ApiArticleClass> ApiArticleClasses => Set<ApiArticleClass>();
    public DbSet<ApiArticleFamily> ApiArticleFamilies => Set<ApiArticleFamily>();
    public DbSet<ApiArticleSubfamily> ApiArticleSubfamilies => Set<ApiArticleSubfamily>();
    public DbSet<ApiArticleType> ApiArticleTypes => Set<ApiArticleType>();
    public DbSet<ApiMeasurementUnit> ApiMeasurementUnits => Set<ApiMeasurementUnit>();
    public DbSet<ApiSizeUnit> ApiSizeUnits => Set<ApiSizeUnit>();
    public DbSet<ApiVatRate> ApiVatRates => Set<ApiVatRate>();
    public DbSet<ApiArticle> ApiArticles => Set<ApiArticle>();
    public DbSet<ApiWarehouse> ApiWarehouses => Set<ApiWarehouse>();
    public DbSet<ApiWarehouseLocation> ApiWarehouseLocations => Set<ApiWarehouseLocation>();
    public DbSet<ApiWarehouseArticle> ApiWarehouseArticles => Set<ApiWarehouseArticle>();
    public DbSet<ApiStockMovement> ApiStockMovements => Set<ApiStockMovement>();
    public DbSet<ApiStockMovementItem> ApiStockMovementItems => Set<ApiStockMovementItem>();
    public DbSet<ApiOrder> ApiOrders => Set<ApiOrder>();
    public DbSet<ApiOrderTicket> ApiOrderTickets => Set<ApiOrderTicket>();
    public DbSet<ApiOrderDetail> ApiOrderDetails => Set<ApiOrderDetail>();
    public DbSet<ApiArticleChild> ApiArticleChildren => Set<ApiArticleChild>();
    public DbSet<ApiPaymentMethod> ApiPaymentMethods => Set<ApiPaymentMethod>();
    public DbSet<ApiFiscalYear> ApiFiscalYears => Set<ApiFiscalYear>();
    public DbSet<ApiDocumentType> ApiDocumentTypes => Set<ApiDocumentType>();
    public DbSet<ApiDocumentSeries> ApiDocumentSeries => Set<ApiDocumentSeries>();
    public DbSet<ApiDocument> ApiDocuments => Set<ApiDocument>();
    public DbSet<ApiDocumentDetail> ApiDocumentDetails => Set<ApiDocumentDetail>();
    public DbSet<ApiDocumentPaymentMethod> ApiDocumentPaymentMethods => Set<ApiDocumentPaymentMethod>();

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

        modelBuilder.Entity<ApiMovementType>(entity =>
        {
            entity.HasKey(movementType => movementType.Id);
            entity.Property(movementType => movementType.Code).HasMaxLength(64).IsRequired();
            entity.Property(movementType => movementType.Designation).HasMaxLength(256).IsRequired();
            entity.Property(movementType => movementType.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(movementType => movementType.CreatedUtc).IsRequired();
            entity.Property(movementType => movementType.UpdatedUtc).IsRequired();
        });

        modelBuilder.Entity<ApiPlace>(entity =>
        {
            entity.HasKey(place => place.Id);
            entity.Property(place => place.Code).HasMaxLength(64).IsRequired();
            entity.Property(place => place.Designation).HasMaxLength(256).IsRequired();
            entity.Property(place => place.ButtonImage).HasMaxLength(512);
            entity.Property(place => place.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(place => place.CreatedUtc).IsRequired();
            entity.Property(place => place.UpdatedUtc).IsRequired();
            entity.HasOne<ApiMovementType>()
                .WithMany()
                .HasForeignKey(place => place.MovementTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApiTable>(entity =>
        {
            entity.HasKey(table => table.Id);
            entity.Property(table => table.Code).HasMaxLength(64).IsRequired();
            entity.Property(table => table.Designation).HasMaxLength(256).IsRequired();
            entity.Property(table => table.ButtonImage).HasMaxLength(512);
            entity.Property(table => table.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(table => table.CreatedUtc).IsRequired();
            entity.Property(table => table.UpdatedUtc).IsRequired();
            entity.HasOne<ApiPlace>()
                .WithMany()
                .HasForeignKey(table => table.PlaceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApiHoliday>(entity =>
        {
            entity.HasKey(holiday => holiday.Id);
            entity.Property(holiday => holiday.Code).HasMaxLength(64).IsRequired();
            entity.Property(holiday => holiday.Designation).HasMaxLength(256).IsRequired();
            entity.Property(holiday => holiday.Description).HasMaxLength(512).IsRequired();
            entity.Property(holiday => holiday.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(holiday => holiday.CreatedUtc).IsRequired();
            entity.Property(holiday => holiday.UpdatedUtc).IsRequired();
        });

        modelBuilder.Entity<ApiArticleClass>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Code).HasMaxLength(64).IsRequired();
            entity.Property(item => item.Designation).HasMaxLength(256).IsRequired();
            entity.Property(item => item.Acronym).HasMaxLength(16).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.Property(item => item.UpdatedUtc).IsRequired();
        });

        modelBuilder.Entity<ApiArticleFamily>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Code).HasMaxLength(64).IsRequired();
            entity.Property(item => item.Designation).HasMaxLength(256).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.Property(item => item.UpdatedUtc).IsRequired();
        });

        modelBuilder.Entity<ApiArticleSubfamily>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Code).HasMaxLength(64).IsRequired();
            entity.Property(item => item.Designation).HasMaxLength(256).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.Property(item => item.UpdatedUtc).IsRequired();
            entity.HasOne<ApiArticleFamily>()
                .WithMany()
                .HasForeignKey(item => item.FamilyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApiArticleType>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Code).HasMaxLength(64).IsRequired();
            entity.Property(item => item.Designation).HasMaxLength(256).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.Property(item => item.UpdatedUtc).IsRequired();
        });

        modelBuilder.Entity<ApiMeasurementUnit>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Code).HasMaxLength(64).IsRequired();
            entity.Property(item => item.Designation).HasMaxLength(256).IsRequired();
            entity.Property(item => item.Acronym).HasMaxLength(16).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.Property(item => item.UpdatedUtc).IsRequired();
        });

        modelBuilder.Entity<ApiSizeUnit>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Code).HasMaxLength(64).IsRequired();
            entity.Property(item => item.Designation).HasMaxLength(256).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.Property(item => item.UpdatedUtc).IsRequired();
        });

        modelBuilder.Entity<ApiVatRate>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Code).HasMaxLength(64).IsRequired();
            entity.Property(item => item.Designation).HasMaxLength(256).IsRequired();
            entity.Property(item => item.ReasonCode).HasMaxLength(64).IsRequired();
            entity.Property(item => item.TaxType).HasMaxLength(64).IsRequired();
            entity.Property(item => item.TaxCode).HasMaxLength(64).IsRequired();
            entity.Property(item => item.CountryRegion).HasMaxLength(64).IsRequired();
            entity.Property(item => item.Description).HasMaxLength(512).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.Property(item => item.UpdatedUtc).IsRequired();
        });

        modelBuilder.Entity<ApiArticle>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Code).HasMaxLength(64).IsRequired();
            entity.Property(item => item.Designation).HasMaxLength(256).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.Property(item => item.UpdatedUtc).IsRequired();
            entity.HasOne<ApiArticleClass>()
                .WithMany()
                .HasForeignKey(item => item.ClassId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApiArticleSubfamily>()
                .WithMany()
                .HasForeignKey(item => item.SubfamilyId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApiArticleType>()
                .WithMany()
                .HasForeignKey(item => item.TypeId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApiMeasurementUnit>()
                .WithMany()
                .HasForeignKey(item => item.MeasurementUnitId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApiSizeUnit>()
                .WithMany()
                .HasForeignKey(item => item.SizeUnitId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApiVatRate>()
                .WithMany()
                .HasForeignKey(item => item.VatDirectSellingId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApiWarehouse>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Code).HasMaxLength(64).IsRequired();
            entity.Property(item => item.Designation).HasMaxLength(256).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.Property(item => item.UpdatedUtc).IsRequired();
        });

        modelBuilder.Entity<ApiWarehouseLocation>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Designation).HasMaxLength(256).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.Property(item => item.UpdatedUtc).IsRequired();
            entity.HasOne<ApiWarehouse>()
                .WithMany()
                .HasForeignKey(item => item.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApiWarehouseArticle>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.Property(item => item.UpdatedUtc).IsRequired();
            entity.HasOne<ApiArticle>()
                .WithMany()
                .HasForeignKey(item => item.ArticleId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApiWarehouseLocation>()
                .WithMany()
                .HasForeignKey(item => item.WarehouseLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApiStockMovement>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.DocumentNumber).HasMaxLength(128).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.Property(item => item.UpdatedUtc).IsRequired();
            entity.HasMany(item => item.Items)
                .WithOne()
                .HasForeignKey(item => item.StockMovementId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ApiStockMovementItem>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.HasOne<ApiArticle>()
                .WithMany()
                .HasForeignKey(item => item.ArticleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApiOrder>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.Property(item => item.UpdatedUtc).IsRequired();
            entity.HasOne<ApiTable>()
                .WithMany()
                .HasForeignKey(item => item.TableId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(item => item.Tickets)
                .WithOne()
                .HasForeignKey(item => item.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ApiOrderTicket>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.HasMany(item => item.Details)
                .WithOne()
                .HasForeignKey(item => item.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ApiOrderDetail>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Designation).HasMaxLength(256).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.Property(item => item.UpdatedUtc).IsRequired();
            entity.HasOne<ApiArticle>()
                .WithMany()
                .HasForeignKey(item => item.ArticleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApiArticleChild>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.HasIndex(item => new { item.ParentArticleId, item.ChildArticleId }).IsUnique();
            entity.HasOne<ApiArticle>()
                .WithMany()
                .HasForeignKey(item => item.ParentArticleId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<ApiArticle>()
                .WithMany()
                .HasForeignKey(item => item.ChildArticleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApiPaymentMethod>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Code).HasMaxLength(64).IsRequired();
            entity.Property(item => item.Designation).HasMaxLength(256).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.Property(item => item.UpdatedUtc).IsRequired();
        });

        modelBuilder.Entity<ApiFiscalYear>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Code).HasMaxLength(64).IsRequired();
            entity.Property(item => item.Designation).HasMaxLength(256).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.Property(item => item.UpdatedUtc).IsRequired();
        });

        modelBuilder.Entity<ApiDocumentType>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.HasIndex(item => item.Acronym).IsUnique();
            entity.Property(item => item.Code).HasMaxLength(64).IsRequired();
            entity.Property(item => item.Designation).HasMaxLength(256).IsRequired();
            entity.Property(item => item.Acronym).HasMaxLength(16).IsRequired();
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.Property(item => item.UpdatedUtc).IsRequired();
        });

        modelBuilder.Entity<ApiDocumentSeries>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Code).HasMaxLength(64).IsRequired();
            entity.Property(item => item.Designation).HasMaxLength(256).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.Property(item => item.UpdatedUtc).IsRequired();
            entity.HasOne<ApiDocumentType>()
                .WithMany()
                .HasForeignKey(item => item.DocumentTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApiFiscalYear>()
                .WithMany()
                .HasForeignKey(item => item.FiscalYearId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApiDocument>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Type).HasMaxLength(16).IsRequired();
            entity.Property(item => item.Number).HasMaxLength(64).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1024).IsRequired();
            entity.Property(item => item.CreatedUtc).IsRequired();
            entity.Property(item => item.UpdatedUtc).IsRequired();
            // DocumentSeriesService.IssueNumberAsync increments NextNumber in memory with no row lock, so two
            // concurrent issues on the same series could otherwise both claim the same number. This unique
            // index (drafts excluded — they all share Number = "") turns that race into a clean, visible
            // failure (caught by the DbUpdateException handler in Program.cs) instead of silently producing
            // two fiscal documents with an identical sequential number.
            entity.HasIndex(item => item.Number).IsUnique().HasFilter("Number <> ''");
            entity.HasOne<ApiDocumentSeries>()
                .WithMany()
                .HasForeignKey(item => item.DocumentSeriesId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(item => item.Details)
                .WithOne()
                .HasForeignKey(item => item.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(item => item.PaymentMethods)
                .WithOne()
                .HasForeignKey(item => item.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ApiDocumentDetail>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Code).HasMaxLength(64).IsRequired();
            entity.Property(item => item.Designation).HasMaxLength(256).IsRequired();
            entity.HasOne<ApiArticle>()
                .WithMany()
                .HasForeignKey(item => item.ArticleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApiDocumentPaymentMethod>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.HasOne<ApiPaymentMethod>()
                .WithMany()
                .HasForeignKey(item => item.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

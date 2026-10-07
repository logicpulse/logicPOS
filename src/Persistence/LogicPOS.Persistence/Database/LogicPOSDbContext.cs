using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Entities.POS.Orders.Orders.Common;
using LogicPOS.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace LogicPOS.Persistence.Database;

public class LogicPOSDbContext : DbContext
{
    private readonly AuditingInterceptor _auditingInterceptor;

    public LogicPOSDbContext(DbContextOptions<LogicPOSDbContext> options,
        AuditingInterceptor auditingInterceptor) :
        base(options)
    {
        _auditingInterceptor = auditingInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditingInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LogicPOSDbContext).Assembly);
    }

    public bool DatabaseExists()
    {
        IRelationalDatabaseCreator? creator =
            this.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
        if (creator == null)
        {
            return false;
        }

        return creator.Exists() && creator.HasTables();
    }

    #region Tables

    public DbSet<Article> Articles { get; private set; } = null!;
    public DbSet<ArticleClass> ArticleClasses { get; private set; } = null!;
    public DbSet<ArticleComposition> ArticleCompositions { get; private set; } = null!;
    public DbSet<ArticleFamily> ArticleFamilies { get; private set; } = null!;
    public DbSet<ArticleSubfamily> ArticleSubfamilies { get; private set; } = null!;
    public DbSet<StockMovement> StockMovements { get; private set; } = null!;
    public DbSet<ArticleType> ArticleTypes { get; private set; } = null!;
    public DbSet<WarehouseArticle> WarehouseArticles { get; private set; } = null!;
    public DbSet<UniqueArticleComposition> UniqueArticleCompositions { get; private set; } = null!;
    public DbSet<MeasurementUnit> MeasurementUnits { get; private set; } = null!;
    public DbSet<SizeUnit> SizeUnits { get; private set; } = null!;
    public DbSet<Country> Countries { get; private set; } = null!;
    public DbSet<Currency> Currencies { get; private set; } = null!;
    public DbSet<Holiday> Holidays { get; private set; } = null!;
    public DbSet<Place> Places { get; private set; } = null!;
    public DbSet<PreferenceParameter> PreferenceParameters { get; private set; } = null!;
    public DbSet<Customer> Customers { get; private set; } = null!;
    public DbSet<DiscountGroup> DiscountGroups { get; private set; } = null!;
    public DbSet<CustomerType> CustomerTypes { get; private set; } = null!;
    public DbSet<DocumentType> DocumentTypes { get; private set; } = null!;
    public DbSet<OrderDetail> OrderDetails { get; private set; } = null!;
    public DbSet<Order> Orders { get; private set; } = null!;
    public DbSet<OrderDocument> OrderDocuments { get; private set; } = null!;
    public DbSet<Ticket> Tickets { get; private set; } = null!;
    public DbSet<Commission> Commissions { get; private set; } = null!;
    public DbSet<DocumentDetail> DocumentDetails { get; private set; } = null!;
    public DbSet<Document> Documents { get; private set; } = null!;
    public DbSet<Payment> Payments { get; private set; } = null!;
    public DbSet<Receipt> Receipts { get; private set; } = null!;
    public DbSet<DocumentSeries> DocumentSeries { get; private set; } = null!;
    public DbSet<FiscalYear> FiscalYears { get; private set; } = null!;
    public DbSet<PaymentCondition> PaymentConditions { get; private set; } = null!;
    public DbSet<PaymentMethod> PaymentMethods { get; private set; } = null!;
    public DbSet<Domain.Entities.PriceType> PriceTypes { get; private set; } = null!;
    public DbSet<VatExemptionReason> VatExemptionReasons { get; private set; } = null!;
    public DbSet<VatRate> VatRates { get; private set; } = null!;
    public DbSet<Warehouse> Warehouses { get; private set; } = null!;
    public DbSet<WarehouseLocation> WarehouseLocations { get; private set; } = null!;
    public DbSet<InputReader> InputReaders { get; private set; } = null!;
    public DbSet<PoleDisplay> PoleDisplays { get; private set; } = null!;
    public DbSet<PrinterType> PrinterTypes { get; private set; } = null!;
    public DbSet<Printer> Printers { get; private set; } = null!;
    public DbSet<WeighingMachine> WeighingMachines { get; private set; } = null!;
    public DbSet<MovementType> MovementTypes { get; private set; } = null!;
    public DbSet<Table> Tables { get; private set; } = null!;
    public DbSet<Terminal> Terminals { get; private set; } = null!;
    public DbSet<CommissionGroup> CommissionGroups { get; private set; } = null!;
    public DbSet<User> Users { get; private set; } = null!;
    public DbSet<UserVerificationCode> UserVerificationCodes { get; private set; } = null!;
    public DbSet<SmsOutboundMessage> SmsOutboundMessages { get; private set; } = null!;
    public DbSet<PermissionGroup> PermissionGroups { get; private set; } = null!;
    public DbSet<PermissionItem> PermissionItems { get; private set; } = null!;
    public DbSet<PermissionProfile> PermissionProfiles { get; private set; } = null!;
    public DbSet<UserProfile> UserProfiles { get; private set; } = null!;
    public DbSet<WorkSessionMovement> WorkSessionMovements { get; private set; } = null!;
    public DbSet<WorkSessionPeriod> WorkSessionPeriods { get; private set; } = null!;
    public DbSet<DocumentPaymentMethod> DocumentPaymentMethods { get; private set; } = null!;
    public DbSet<PrinterAssociation> PrinterAssociations { get; private set; } = null!;
    public DbSet<ATAudit> AtAudits { get; private set; } = null!;
    public DbSet<SystemAuditType> SystemAuditTypes { get; private set; } = null!;
    public DbSet<SystemAudit> SystemAudits { get; private set; } = null!;
    public DbSet<DocumentPrint> DocumentPrints { get; private set; } = null!;
    public DbSet<SystemBackup> SystemBackups { get; private set; } = null!;
    public DbSet<SystemNotification> SystemNotifications { get; private set; } = null!;
    public DbSet<SystemNotificationType> SystemNotificationTypes { get; private set; } = null!;
    public DbSet<DocumentNotification> DocumentNotifications { get; private set; } = null!;

    #endregion
}
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Entities.POS.Devices.Printers.Printer;
using LogicPOS.Persistence.Entities.POS.WorkSessions.Movements;
using LogicPOS.Persistence.Entities.POS.WorkSessions.Periods;
using LogicPOS.Persistence.Entities.System.Users.Commissions;
using LogicPOS.Persistence.Repositories;

using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Persistence.DependencyInjection;

internal static class RepositoriesDependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<ICommissionGroupRepository, CommissionGroupRepository>();
        services.AddScoped<IPermissionGroupRepository, PermissionGroupRepository>();
        services.AddScoped<IWarehouseLocationRepository, WarehouseLocationRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IHolidayRepository, HolidayRepository>();
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<IWeighingMachineRepository, WeighingMachineRepository>();
        services.AddScoped<IVatRateRepository, VatRateRepository>();
        services.AddScoped<IPriceTypeRepository, PriceTypeRepository>();
        services.AddScoped<ICustomerTypeRepository, CustomerTypeRepository>();
        services.AddScoped<IDiscountGroupRepository, DiscountGroupRepository>();
        services.AddScoped<IFiscalYearRepository, FiscalYearRepository>();
        services.AddScoped<IVatRateRepository, VatRateRepository>();
        services.AddScoped<IVatExemptionReasonRepository, VatExemptionReasonRepository>();
        services.AddScoped<IArticleClassRepository, ArticleClassRepository>();
        services.AddScoped<IArticleTypeRepository, ArticleTypeRepository>();
        services.AddScoped<IPrinterTypeRepository, PrinterTypeRepository>();
        services.AddScoped<IMovementTypeRepository, MovementTypeRepository>();
        services.AddScoped<IPreferenceParameterRepository, PreferenceParameterRepository>();
        services.AddScoped<IPrinterRepository, PrinterRepository>();
        services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
        services.AddScoped<IArticleFamilyRepository, ArticleFamilyRepository>();
        services.AddScoped<IArticleSubfamilyRepository, ArticleSubfamilyRepository>();
        services.AddScoped<ISizeUnitRepository, SizeUnitRepository>();
        services.AddScoped<IMeasurementUnitRepository, MeasurementUnitRepository>();
        services.AddScoped<IPlaceRepository, PlaceRepository>();
        services.AddScoped<IArticleRepository, ArticleRepository>();
        services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
        services.AddScoped<IInputReaderRepository, InputReaderRepository>();
        services.AddScoped<IPaymentConditionRepository, PaymentConditionRepository>();
        services.AddScoped<ITableRepository, TableRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
        services.AddScoped<IPoleDisplayRepository, PoleDisplayRepository>();
        services.AddScoped<ITerminalRepository, TerminalRepository>();
        services.AddScoped<IWorkSessionPeriodRepository, WorkSessionPeriodRepository>();
        services.AddScoped<IDocumentSeriesRepository, DocumentSeriesRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IDocumentDetailRepository, DocumentDetailRepository>();
        services.AddScoped<IReceiptRepository, ReceiptRepository>();
        services.AddScoped<IPermissionItemRepository, PermissionItemRepository>();
        services.AddScoped<IPermissionProfileRepository, PermissionProfileRepository>();
        services.AddScoped<IArticleCompositionRepository, ArticleCompositionRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IWorkSessionMovementRepository, WorkSessionMovementRepository>();
        services.AddScoped<IPrinterAssociationRepository, PrinterAssociationRepository>();

        return services;
    }
}
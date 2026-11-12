using System.Globalization;
using LogicPOS.Application.Features.Articles.Stocks;
using LogicPOS.Application.Features.System;
using LogicPOS.Core.Authentication;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.Fiscal;
using LogicPOS.Core.Licensing;
using LogicPOS.Core.FrontOffice;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.DependencyInjection;
using LogicPOS.Persistence.Interceptors;
using LogicPOS.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Core;

public static class AppComposition
{
    public static IServiceProvider? Services { get; private set; }

    public static string? StartupError { get; private set; }

    /// <summary>
    /// Lets the LogicPulse host attach its own composition so Avalonia UI can resolve services.
    /// </summary>
    public static void UseExternal(IServiceProvider? services, string? startupError = null)
    {
        Services = services;
        StartupError = startupError;
    }

    public static void Configure() => Configure(AppContext.BaseDirectory);

    public static void Configure(string baseDirectory)
    {
        StartupError = null;
        Services = null;
        try
        {
            var settingsPath = Path.Combine(baseDirectory, "appsettings.json");
            if (File.Exists(settingsPath) == false)
            {
                StartupError = "Falta appsettings.json ao lado da aplicação.";
                return;
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(baseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            configuration = new ConfigurationBuilder()
                .SetBasePath(baseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["DatabaseSettings:UseCloud"] = "false",
                    ["DatabaseSettings:SeedPath"] = DatabaseStartup.ResolveSeedPath(baseDirectory),
                    // Honour appsettings (false = no demo articles/tables/customers on a fresh DB).
                    ["DatabaseSettings:UseSeed"] = configuration["DatabaseSettings:UseSeed"] ?? "true",
                    // Module comes from appsettings.json (e.g. restaurant, cafe, default).
                    ["DatabaseSettings:Module"] = configuration["DatabaseSettings:Module"] ?? "default"
                })
                .Build();

            var culture = CultureInfo.GetCultureInfo(UiCulture.Read(configuration["Culture"]));
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            var databaseSettings = DatabaseSettings.GetFromConfiguration(configuration);
            if (databaseSettings.IsValid() == false)
            {
                StartupError = "A configuração da base de dados é inválida. Preenche ConnectionString em appsettings.json.";
                return;
            }

            AppLogging.Configure(baseDirectory);
            AppLogging.Information("Initializing application (local)...");

            var services = new ServiceCollection();
            AppLogging.AddToServices(services);
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<ILicenseModule, NullLicenseModule>();
            services.AddSingleton<ISystemInformationService, DesktopSystemInformationService>();
            services.AddSingleton<IAuditingInformationService, DesktopAuditingInformationService>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddPersistence(configuration);
            services.AddSingleton<LoginService>();
            services.AddSingleton<ILoginService>(provider => provider.GetRequiredService<LoginService>());
            services.AddSingleton<IOperationMode, ConfigurationOperationMode>();
            services.AddSingleton<PosCatalogService>();
            services.AddSingleton<IPosCatalogService>(provider => provider.GetRequiredService<PosCatalogService>());
            services.AddSingleton<PosCustomerService>();
            services.AddSingleton<IPosCustomerService>(provider => provider.GetRequiredService<PosCustomerService>());
            services.AddScoped<IDocumentHasher, LocalDocumentHasher>();
            services.AddSingleton<PosDocumentService>();
            services.AddSingleton<IPosDocumentService>(provider => provider.GetRequiredService<PosDocumentService>());
            services.AddSingleton<PosOrderService>();
            services.AddSingleton<IPosOrderService>(provider => provider.GetRequiredService<PosOrderService>());
            services.AddSingleton<BackOfficeListingService>();
            services.AddSingleton<IBackOfficeListingService>(provider => provider.GetRequiredService<BackOfficeListingService>());
            services.AddSingleton<IBulkEditService, BulkEditService>();
            services.AddSingleton<LocalStockService>();
            services.AddSingleton<IStockManagementService>(provider => provider.GetRequiredService<LocalStockService>());
            services.AddSingleton<LocalDocumentStockProcessor>();
            services.AddSingleton<IDocumentStockProcessor>(
                provider => provider.GetRequiredService<LocalDocumentStockProcessor>());
            services.AddSingleton<IReceiptEmission, LocalReceiptEmission>();
            services.AddSingleton<DashboardBillingService>();
            services.AddSingleton<IDashboardBillingService>(provider => provider.GetRequiredService<DashboardBillingService>());
            services.AddSingleton<LocalReportService>();
            services.AddSingleton<IReportService>(provider => provider.GetRequiredService<LocalReportService>());
            services.AddSingleton<LocalFiscalYearWizard>();
            services.AddSingleton<IFiscalYearWizard>(provider => provider.GetRequiredService<LocalFiscalYearWizard>());
            services.AddSingleton<LocalArticleSetupWizard>();
            services.AddSingleton<IArticleSetupWizard>(provider => provider.GetRequiredService<LocalArticleSetupWizard>());
            services.AddSingleton<LocalFinalConsumerSetupWizard>();
            services.AddSingleton<IFinalConsumerSetupWizard>(provider => provider.GetRequiredService<LocalFinalConsumerSetupWizard>());
            services.AddSingleton<LocalTableSetupWizard>();
            services.AddSingleton<ITableSetupWizard>(provider => provider.GetRequiredService<LocalTableSetupWizard>());
            services.AddSingleton<LocalPrinterSetupWizard>();
            services.AddSingleton<IPrinterSetupWizard>(provider => provider.GetRequiredService<LocalPrinterSetupWizard>());
            services.AddSingleton<LocalCompanySetupService>();
            services.AddSingleton<ICompanySetupService>(provider => provider.GetRequiredService<LocalCompanySetupService>());
            services.AddSingleton<LocalFirstUserSetupWizard>();
            services.AddSingleton<IFirstUserSetupWizard>(provider => provider.GetRequiredService<LocalFirstUserSetupWizard>());
            services.AddSingleton<StartupReadinessService>();
            services.AddSingleton<IStartupReadinessService>(provider => provider.GetRequiredService<StartupReadinessService>());
            services.AddSingleton<OnboardingService>();
            services.AddSingleton<IOnboardingService>(provider => provider.GetRequiredService<OnboardingService>());
            services.AddSingleton<CompanyCurrency>();
            services.AddSingleton<ITicketPrinter, EscPosTicketPrinter>();
            services.AddSingleton<IThermalPrintSource, LocalThermalPrintSource>();
            if (FiscalModuleLoader.TryRegister(services, baseDirectory) == false)
            {
                services.AddSingleton<IFiscalModule, NullFiscalModule>();
            }
            var provider = services.BuildServiceProvider();
            DatabaseStartup.EnsureDatabase(provider, databaseSettings);
            DatabaseStartup.EnsureMachineTerminalAsync(provider).GetAwaiter().GetResult();
            // Do not auto-create fiscal year / series here. Fresh DBs stay empty until the
            // Abertura de ano fiscal wizard (or EnsureSeriesAsync on first document issue).
            Services = provider;
            using var currencyScope = provider.CreateScope();
            provider.GetRequiredService<CompanyCurrency>().Reload(
                currencyScope.ServiceProvider.GetRequiredService<LogicPOS.Persistence.Database.LogicPOSDbContext>());
        }
        catch (Exception exception)
        {
            StartupError = exception.Message;
        }
    }
}

using LogicPOS.Application.Features.System;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using LogicPOS.Persistence.Entities.Articles.Articles;
using LogicPOS.Persistence.Entities.Articles.Classes;
using LogicPOS.Persistence.Entities.Articles.Families;
using LogicPOS.Persistence.Entities.Articles.PriceTypes;
using LogicPOS.Persistence.Entities.Articles.Stocks.Warehouses;
using LogicPOS.Persistence.Entities.Articles.Stocks.Warehouses.Locations;
using LogicPOS.Persistence.Entities.Articles.Subfamilies;
using LogicPOS.Persistence.Entities.Articles.Types;
using LogicPOS.Persistence.Entities.Articles.Units.Measurement;
using LogicPOS.Persistence.Entities.Articles.Units.Size;
using LogicPOS.Persistence.Entities.Finance.Countries;
using LogicPOS.Persistence.Entities.Finance.Currencies;
using LogicPOS.Persistence.Entities.Finance.Customers.Customers;
using LogicPOS.Persistence.Entities.Finance.Customers.Types;
using LogicPOS.Persistence.Entities.Finance.DiscountGroups;
using LogicPOS.Persistence.Entities.Finance.Documents.Types;
using LogicPOS.Persistence.Entities.Finance.Holidays;
using LogicPOS.Persistence.Entities.Finance.PaymentConditions;
using LogicPOS.Persistence.Entities.Finance.PaymentMethods;
using LogicPOS.Persistence.Entities.Finance.VatExemptionReasons;
using LogicPOS.Persistence.Entities.Finance.VatRates;
using LogicPOS.Persistence.Entities.POS.Devices.InputReaders;
using LogicPOS.Persistence.Entities.POS.Devices.PoleDisplays;
using LogicPOS.Persistence.Entities.POS.Devices.Printers.Printers;
using LogicPOS.Persistence.Entities.POS.Devices.Printers.Types;
using LogicPOS.Persistence.Entities.POS.Devices.WeighingMachines;
using LogicPOS.Persistence.Entities.POS.MovementTypes;
using LogicPOS.Persistence.Entities.POS.Places;
using LogicPOS.Persistence.Entities.POS.Tables;
using LogicPOS.Persistence.Entities.System.Audits;
using LogicPOS.Persistence.Entities.System.Notifications;
using LogicPOS.Persistence.Entities.System.PreferenceParameters;
using LogicPOS.Persistence.Entities.System.Users.Commissions;
using LogicPOS.Persistence.Entities.System.Users.Permissions.Groups;
using LogicPOS.Persistence.Entities.System.Users.Permissions.Items;
using LogicPOS.Persistence.Entities.System.Users.Permissions.Profiles;
using LogicPOS.Persistence.Entities.System.Users.Users.Profiles;
using LogicPOS.Persistence.Entities.System.Users.Users.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Seeding;

public sealed class DatabaseSeeder : IDatabaseSeeder
{
    private readonly LogicPOSDbContext _database;
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly IConfiguration _configuration;
    private readonly ISystemInformationService _systemInformationService;
    private string _systemCountryCode2 = null!;
    private DatabaseSettings _dbSettings = new();

    public DatabaseSeeder(ILogger<DatabaseSeeder> logger, LogicPOSDbContext database, IConfiguration configuration,
        ISystemInformationService systemInformationService)
    {
        _logger = logger;
        _database = database;
        _configuration = configuration;
        _systemInformationService = systemInformationService;
        Initialize();
    }

    private void Initialize()
    {
        _dbSettings = DatabaseSettings.GetFromConfiguration(_configuration);
        _systemCountryCode2 = _systemInformationService.GetCountryCode2().ToLower();
        _logger.LogInformation("Using country code: {CountryCode}", _systemCountryCode2);

        _dbSettings.SeedPath ??= "db_seeds";
        _logger.LogInformation("Seeds path: {SeedsPath}", _dbSettings.SeedPath);
        _dbSettings.Module ??= "default";
        _dbSettings.Module = _dbSettings.Module.ToLower();
        _logger.LogInformation("Seed module: {Module}", _dbSettings.Module);
        string[] validModules =
        [
            "default", "parking", "bakery", "butchery", "cafe", "clothingstore", "hardwarestore",
            "restaurant", "seafoodstore", "shoestore"
        ];

        if (validModules.Contains(_dbSettings.Module) is false)
        {
            _logger.LogWarning("Seed module '{Module}' is not valid. Setting it to 'default'", _dbSettings.Module);
            _dbSettings.Module = "default";
        }
    }

    public void ApplyAdditionalSeed()
    {
        _logger.LogInformation("Applying additional database seed");

        ArticleFamiliesSeeder.SeedData(_database, _dbSettings, _logger);
        ArticleTypesSeeder.SeedData(_database, _dbSettings, _logger);
        PriceTypesSeeder.SeedData(_database, _dbSettings, _logger);
        CommissionGroupsSeeder.SeedData(_database, _dbSettings, _logger);
        UserProfilesSeeder.SeedData(_database, _dbSettings, _logger);
        PermissionProfilesSeeder.SeedData(_database, _dbSettings, _logger);
        UsersSeeder.SeedData(_database, _dbSettings, _logger);
        ArticleSubfamiliesSeeder.SeedData(_database, _dbSettings, _logger);
        ArticlesSeeder.SeedData(_database, _dbSettings, _logger);
        PlacesSeeder.SeedData(_database, _dbSettings, _logger);
        TablesSeeder.SeedData(_database, _dbSettings, _logger);
        CustomerTypesSeeder.SeedData(_database, _dbSettings, _logger);
        CustomersSeeder.SeedData(_database, _dbSettings, _systemCountryCode2, _logger);

        _logger.LogInformation("Additional data seeded successfully");
    }

    public bool ApplyRequiredSeed()
    {
        _logger.LogInformation("Applying required seed");
        bool preferenceParameters =
            PreferenceParametersSeeder.SeedData(_database, _dbSettings!, _systemCountryCode2!, _logger);

        if (preferenceParameters == false)
        {
            _logger.LogCritical("Preference parameters seeding failed.");
            return false;
        }

        bool articleClasses = ArticleClassesSeeder.SeedData(_database, _dbSettings, _logger);
        if (articleClasses == false)
        {
            _logger.LogCritical("Article classes seeding failed.");
            return false;
        }
        
        bool countries =  CountriesSeeder.SeedData(_database, _dbSettings, _logger);
        if (countries == false)
        {
            _logger.LogCritical("Countries seeding failed.");
            return false;       
        }
        
        CurrenciesSeeder.SeedData(_database, _dbSettings, _logger);
        PaymentMethodsSeeder.SeedData(_database, _dbSettings, _logger);
        VatRatesSeeder.SeedData(_database, _dbSettings, _systemCountryCode2!, _logger);
        VatExemptionReasonsSeeder.SeedData(_database, _dbSettings, _systemCountryCode2!, _logger);
        WarehousesSeeder.SeedData(_database, _dbSettings, _logger);
        WarehouseLocationsSeeder.SeedData(_database, _dbSettings, _logger);
        HolidaysSeeder.SeedData(_database, _dbSettings, _systemCountryCode2, _logger);
        SystemNotificationsTypesSeeder.SeedData(_database, _dbSettings, _logger);
        SystemAuditTypesSeeder.SeedData(_database, _dbSettings, _logger);
        PermissionGroupsSeeder.SeedData(_database, _dbSettings, _logger);
        PermissionItemsSeeder.SeedData(_database, _dbSettings, _logger);
        DocumentTypesSeeder.SeedData(_database, _dbSettings, _systemCountryCode2, _logger);
        PaymentConditionsSeeder.SeedData(_database, _dbSettings, _logger);
        MeasurementUnitsSeeder.SeedData(_database, _dbSettings, _logger);
        SizeUnitsSeeder.SeedData(_database, _dbSettings, _logger);
        PrinterTypesSeeder.SeedData(_database, _dbSettings, _logger);
        PrintersSeeder.SeedData(_database, _dbSettings, _logger);
        InputReaderSeeder.SeedData(_database, _dbSettings, _logger);
        PoleDisplaysSeeder.SeedData(_database, _dbSettings, _logger);
        WeighingMachinesSeeder.SeedData(_database, _dbSettings, _logger);
        MovementTypesSeeder.SeedData(_database, _dbSettings, _logger);
        DiscountGroupsSeeder.SeedData(_database, _dbSettings, _logger);

        _logger.LogInformation("Required seed applied successfully");
        return true;
    }

    public void ApplyNewSeed()
    {
        _logger.LogInformation("Applying new seed");
        Initialize();

        bool preferenceParameters =
            PreferenceParametersSeeder.SeedNewData(_database, _dbSettings!, _systemCountryCode2!, _logger);

        if (preferenceParameters == false)
        {
            _logger.LogCritical("New preference parameters seeding failed. Skipping seeding process.");
        }

        bool permissionItems = PermissionItemsSeeder.SeedNewData(_database, _dbSettings!, _logger);

        if (permissionItems == false)
        {
            _logger.LogCritical("New permission items seeding failed. Skipping seeding process.");
        }

        bool documentTypes =
            DocumentTypesSeeder.SeedNewData(_database, _dbSettings!, _systemCountryCode2!, _logger);

        if (documentTypes == false)
        {
            _logger.LogCritical("New document types seeding failed. Skipping seeding process.");
        }
    }

    public void ApplyRequiredUsersSeed()
    {
        _logger.LogInformation("Applying required users seed");
        var dbSettings = _dbSettings with {Module = "default"};
        UserProfilesSeeder.SeedData(_database, dbSettings, _logger);
        UsersSeeder.SeedData(_database, dbSettings, _logger);
        PermissionProfilesSeeder.SeedData(_database, dbSettings, _logger);
        _logger.LogInformation("Required users seed applied successfully");
    }

    public void EnsurePortalLogin()
    {
        var activeUsers = _database.Users
            .Include(user => user.Profile)
            .Where(user => user.IsDeleted == false)
            .ToList();

        if (activeUsers.Any(user => PortalLoginAssignment.HasPortalIdentity(user.Login, user.Email)))
            return;

        var proprietorId = PortalLoginAssignment.ChooseProprietor(activeUsers.Select(user =>
            new PortalLoginCandidate(user.Id, user.Name, user.Profile?.Designation)));
        if (proprietorId is null)
        {
            _logger.LogWarning("No proprietor user is available to receive the portal login.");
            return;
        }

        var proprietor = activeUsers.First(user => user.Id == proprietorId.Value);
        var loginOwners = _database.Users
            .Where(user => user.Id != proprietor.Id && user.Login == PortalLoginAssignment.Login)
            .ToList();
        if (loginOwners.Any(user => user.IsDeleted == false))
        {
            _logger.LogWarning(
                "Portal login {Login} is already used. No login was assigned.",
                PortalLoginAssignment.Login);
            return;
        }

        foreach (var deletedOwner in loginOwners)
            deletedOwner.Login = null;

        proprietor.Login = PortalLoginAssignment.Login;
        _database.SaveChanges();
        _logger.LogInformation(
            "Assigned portal login {Login} to proprietor {UserName} ({UserId}).",
            PortalLoginAssignment.Login,
            proprietor.Name,
            proprietor.Id);
    }

    public void EnsureCountrySpecificConfiguration()
    {
        if (!_systemInformationService.IsPortugal())
        {
            return;
        }

        _logger.LogInformation("Ensuring Portugal-specific configuration");
        SdrDepositArticleSeeder.EnsureConfigured(_database, _logger);
    }
}
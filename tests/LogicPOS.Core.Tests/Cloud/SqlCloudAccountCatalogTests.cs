using System.Data.Common;
using LogicPOS.Application.Features.System.Licensing;
using LogicPOS.Persistence.Cloud;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace LogicPOS.Core.Tests.Cloud;

public class SqlCloudAccountCatalogTests : IDisposable
{
    private readonly string _sharedConnectionString =
        $"Data Source=u08-catalog-{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
    private readonly SqliteConnection _keepAlive;

    public SqlCloudAccountCatalogTests()
    {
        SqlCloudAccountCatalog.ResetAvailableDevicesCacheForTests();
        _keepAlive = new SqliteConnection(_sharedConnectionString);
        _keepAlive.Open();
    }

    public void Dispose()
    {
        _keepAlive.Dispose();
        SqlCloudAccountCatalog.ResetAvailableDevicesCacheForTests();
    }

    [Fact]
    public void Find_WithoutAvailableDevicesColumn_ReturnsAccountAndLeavesDeviceCountUnset()
    {
        CreateSchema(includeAvailableDevices: false);
        InsertAccount(availableDevices: null);

        var catalog = CreateCatalog();

        var snapshot = catalog.Find("acme");

        Assert.True(snapshot.Found);
        Assert.Equal("Server=tenant;Database=pos;", snapshot.ConnectionString);
        Assert.Equal(0, snapshot.AvailableDevices);
    }

    [Fact]
    public void Find_WithAvailableDevicesColumn_ReadsDeviceCount()
    {
        CreateSchema(includeAvailableDevices: true);
        InsertAccount(availableDevices: 7);

        var catalog = CreateCatalog();

        var snapshot = catalog.Find("acme");

        Assert.True(snapshot.Found);
        Assert.Equal(7, snapshot.AvailableDevices);
    }

    [Fact]
    public void Find_WithNullAvailableDevices_ReturnsZero()
    {
        CreateSchema(includeAvailableDevices: true);
        InsertAccount(availableDevices: null, forceNullDevicesColumn: true);

        var catalog = CreateCatalog();

        var snapshot = catalog.Find("acme");

        Assert.True(snapshot.Found);
        Assert.Equal(0, snapshot.AvailableDevices);
    }

    [Fact]
    public void Find_WhenSqlFailsForOtherReason_PropagatesInsteadOfNotFound()
    {
        CreateSchema(includeAvailableDevices: true);
        InsertAccount(availableDevices: 3);

        var catalog = CreateCatalog(connectionFactory: _ =>
            throw new TimeoutException("simulated catalog timeout"));

        var ex = Assert.Throws<TimeoutException>(() => catalog.Find("acme"));
        Assert.Equal("simulated catalog timeout", ex.Message);
    }

    [Fact]
    public void Find_AfterMissingColumn_CachesOmissionAndStillFindsAccount()
    {
        CreateSchema(includeAvailableDevices: false);
        InsertAccount(availableDevices: null);

        var catalog = CreateCatalog();

        Assert.True(catalog.Find("acme").Found);
        catalog.Invalidate("acme");
        Assert.True(catalog.Find("acme").Found);
        Assert.Equal(0, catalog.Find("acme").AvailableDevices);
    }

    [Fact]
    public void Find_UnknownAccount_ReturnsNotFound()
    {
        CreateSchema(includeAvailableDevices: true);
        InsertAccount(availableDevices: 2);

        var catalog = CreateCatalog();

        var snapshot = catalog.Find("unknown-client");

        Assert.False(snapshot.Found);
        Assert.Same(CloudAccountSnapshot.NotFound, snapshot);
    }

    [Fact]
    public void CloudAccountLicense_ZeroDevices_LeavesLocalQuotaFallbackSignal()
    {
        // Public desktop has no EffectiveLicenseData type; the contract is the same as
        // internal EffectiveLicenseData.AllNumberOfDevices: cloud 0 → keep local quota.
        CreateSchema(includeAvailableDevices: false);
        InsertAccount(availableDevices: null);

        var snapshot = CreateCatalog().Find("acme");
        var cloud = new CloudAccountLicense(
            IsActive: snapshot.Found,
            ModuleStocks: snapshot.ModuleStocks,
            ModuleOnlineInvoicing: snapshot.ModuleOnlineInvoicing,
            ModuleSms: snapshot.ModuleSms,
            Demo: snapshot.Demo,
            NumberOfDevices: snapshot.AvailableDevices);

        const int localDevices = 4;
        var effective = cloud.NumberOfDevices > 0 ? cloud.NumberOfDevices : localDevices;
        Assert.Equal(0, cloud.NumberOfDevices);
        Assert.Equal(localDevices, effective);
    }

    private SqlCloudAccountCatalog CreateCatalog(Func<string, DbConnection>? connectionFactory = null)
    {
        return new SqlCloudAccountCatalog(
            new DatabaseSettings
            {
                UseCloud = true,
                CloudConnectionString = _sharedConnectionString
            },
            new MemoryCache(new MemoryCacheOptions()),
            NullLogger<SqlCloudAccountCatalog>.Instance,
            connectionFactory ?? (_ => new SqliteConnection(_sharedConnectionString)));
    }

    private void CreateSchema(bool includeAvailableDevices)
    {
        using var connection = new SqliteConnection(_sharedConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        var devices = includeAvailableDevices ? ", AvailableDevices INTEGER" : string.Empty;
        command.CommandText = $"""
            CREATE TABLE Countries (id INTEGER PRIMARY KEY, name TEXT);
            CREATE TABLE Accounts (
                ConnectionString TEXT,
                ModuleStocks INTEGER,
                ModuleOnlineInvoicing INTEGER,
                ModuleSMS INTEGER,
                Demo INTEGER,
                PosOperationMode TEXT,
                Login TEXT,
                Password TEXT,
                state INTEGER,
                AccountType_id TEXT,
                ExpirationDate TEXT,
                Account_id TEXT,
                Client_id TEXT,
                idCountry INTEGER
                {devices}
            );
            INSERT INTO Countries (id, name) VALUES (1, 'Portugal');
            """;
        command.ExecuteNonQuery();
    }

    private void InsertAccount(int? availableDevices, bool forceNullDevicesColumn = false)
    {
        using var connection = new SqliteConnection(_sharedConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        if (forceNullDevicesColumn)
        {
            command.CommandText = """
                INSERT INTO Accounts (
                    ConnectionString, ModuleStocks, ModuleOnlineInvoicing, ModuleSMS, Demo,
                    PosOperationMode, Login, Password, state, AccountType_id, ExpirationDate,
                    Account_id, Client_id, idCountry, AvailableDevices)
                VALUES (
                    'Server=tenant;Database=pos;', 1, 0, 0, 0,
                    'retail', 'admin', 'secret', 1, @AccountType, @Expiration,
                    'acme', 'acme', 1, NULL);
                """;
        }
        else if (availableDevices is null)
        {
            command.CommandText = """
                INSERT INTO Accounts (
                    ConnectionString, ModuleStocks, ModuleOnlineInvoicing, ModuleSMS, Demo,
                    PosOperationMode, Login, Password, state, AccountType_id, ExpirationDate,
                    Account_id, Client_id, idCountry)
                VALUES (
                    'Server=tenant;Database=pos;', 1, 0, 0, 0,
                    'retail', 'admin', 'secret', 1, @AccountType, @Expiration,
                    'acme', 'acme', 1);
                """;
        }
        else
        {
            command.CommandText = """
                INSERT INTO Accounts (
                    ConnectionString, ModuleStocks, ModuleOnlineInvoicing, ModuleSMS, Demo,
                    PosOperationMode, Login, Password, state, AccountType_id, ExpirationDate,
                    Account_id, Client_id, idCountry, AvailableDevices)
                VALUES (
                    'Server=tenant;Database=pos;', 1, 0, 0, 0,
                    'retail', 'admin', 'secret', 1, @AccountType, @Expiration,
                    'acme', 'acme', 1, @Devices);
                """;
            var devices = command.CreateParameter();
            devices.ParameterName = "@Devices";
            devices.Value = availableDevices.Value;
            command.Parameters.Add(devices);
        }

        var accountType = command.CreateParameter();
        accountType.ParameterName = "@AccountType";
        accountType.Value = CloudAccountTypes.LogicPosCloudService;
        command.Parameters.Add(accountType);

        var expiration = command.CreateParameter();
        expiration.ParameterName = "@Expiration";
        expiration.Value = DateTime.Today.AddYears(1);
        command.Parameters.Add(expiration);

        command.ExecuteNonQuery();
    }
}

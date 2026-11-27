using System.Data.Common;
using System.Reflection;
using LogicPOS.Application.Features.System.Licensing;
using LogicPOS.Persistence.Cloud;
using LogicPOS.Persistence.Database;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using MySqlConnector;

namespace LogicPOS.Core.Tests.Cloud;

public class SqlCloudAccountCatalogTests : IDisposable
{
    private readonly string _sharedConnectionString =
        $"Data Source=u08-catalog-{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
    private readonly SqliteConnection _keepAlive;
    private DateTimeOffset _utcNow = new(2026, 11, 27, 12, 0, 0, TimeSpan.Zero);

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
    public void Find_AfterMissingColumn_CacheExpiresAndReadsNewColumnWithoutRestart()
    {
        CreateSchema(includeAvailableDevices: false);
        InsertAccount(availableDevices: null);

        var catalog = CreateCatalog();
        Assert.True(catalog.Find("acme").Found);
        Assert.Equal(0, catalog.Find("acme").AvailableDevices);

        using (var connection = new SqliteConnection(_sharedConnectionString))
        {
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = """
                ALTER TABLE Accounts ADD COLUMN AvailableDevices INTEGER;
                UPDATE Accounts SET AvailableDevices = 7 WHERE Account_id = 'acme';
                """;
            command.ExecuteNonQuery();
        }

        catalog.Invalidate("acme");
        Assert.Equal(0, catalog.Find("acme").AvailableDevices);

        _utcNow = _utcNow.Add(SqlCloudAccountCatalog.MissingColumnCacheTtl).AddSeconds(1);
        catalog.Invalidate("acme");

        var afterExpiry = catalog.Find("acme");
        Assert.True(afterExpiry.Found);
        Assert.Equal(7, afterExpiry.AvailableDevices);
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
        Assert.True(snapshot.Found);

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

    [Theory]
    [InlineData("SQLite Error 1: 'no such column: a.AvailableDevices'.", true)]
    [InlineData("SQLite Error 1: 'no such column: AvailableDevices'.", true)]
    [InlineData("SQLite Error 1: 'no such column: a.PosOperationMode'.", false)]
    [InlineData("SQLite Error 1: 'no such table: Accounts'.", false)]
    public void Detector_Sqlite_RequiresNoSuchColumnAndAvailableDevices(string message, bool expected)
    {
        var ex = CreateSqliteException(message);
        Assert.Equal(expected, SqlCloudAccountCatalog.IsMissingAvailableDevicesColumn(ex));
    }

    [Theory]
    [InlineData(207, "Invalid column name 'AvailableDevices'.", true)]
    [InlineData(207, "Invalid column name 'PosOperationMode'.", false)]
    [InlineData(208, "Invalid object name 'Accounts'.", false)]
    public void Detector_SqlServer_Requires207AndAvailableDevices(int number, string message, bool expected)
    {
        var ex = CreateSqlException(number, message);
        Assert.Equal(expected, SqlCloudAccountCatalog.IsMissingAvailableDevicesColumn(ex));
    }

    [Theory]
    [InlineData(1054, "Unknown column 'AvailableDevices' in 'field list'", true)]
    [InlineData(1054, "Unknown column 'a.AvailableDevices' in 'field list'", true)]
    [InlineData(1054, "Unknown column 'PosOperationMode' in 'field list'", false)]
    [InlineData(1146, "Table 'Accounts' doesn't exist", false)]
    public void Detector_MySql_Requires1054AndAvailableDevices(int number, string message, bool expected)
    {
        var ex = CreateMySqlException(number, message);
        Assert.Equal(expected, SqlCloudAccountCatalog.IsMissingAvailableDevicesColumn(ex));
    }

    [Fact]
    public void Detector_Sqlite_RealMissingOtherColumn_IsFalse()
    {
        CreateSchema(includeAvailableDevices: true);
        using var connection = new SqliteConnection(_sharedConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT a.NotARealColumn FROM Accounts a LIMIT 1";
        var ex = Assert.Throws<SqliteException>(() => command.ExecuteReader());
        Assert.False(SqlCloudAccountCatalog.IsMissingAvailableDevicesColumn(ex));
    }

    [Fact]
    public void Detector_Sqlite_RealMissingAvailableDevices_IsTrue()
    {
        CreateSchema(includeAvailableDevices: false);
        using var connection = new SqliteConnection(_sharedConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT a.AvailableDevices FROM Accounts a LIMIT 1";
        var ex = Assert.Throws<SqliteException>(() => command.ExecuteReader());
        Assert.True(SqlCloudAccountCatalog.IsMissingAvailableDevicesColumn(ex));
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
            connectionFactory ?? (_ => new SqliteConnection(_sharedConnectionString)),
            utcNow: () => _utcNow);
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

    private static SqliteException CreateSqliteException(string message)
    {
        var ctor = typeof(SqliteException).GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            new[] { typeof(string), typeof(int), typeof(int) },
            null);
        if (ctor is not null)
            return (SqliteException)ctor.Invoke(new object[] { message, 1, 1 });

        ctor = typeof(SqliteException).GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            new[] { typeof(string), typeof(int) },
            null);
        Assert.NotNull(ctor);
        return (SqliteException)ctor!.Invoke(new object[] { message, 1 });
    }

    private static SqlException CreateSqlException(int number, string message)
    {
        var errorCtor = typeof(SqlError).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            new[]
            {
                typeof(int), typeof(byte), typeof(byte), typeof(string), typeof(string),
                typeof(string), typeof(int), typeof(Exception)
            },
            null);
        Assert.NotNull(errorCtor);
        var error = (SqlError)errorCtor!.Invoke(
            new object?[] { number, (byte)16, (byte)1, "server", message, "proc", 1, null });

        var collection = (SqlErrorCollection)Activator.CreateInstance(
            typeof(SqlErrorCollection),
            nonPublic: true)!;
        typeof(SqlErrorCollection)
            .GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(collection, new object[] { error });

        var create = typeof(SqlException).GetMethod(
            "CreateException",
            BindingFlags.Static | BindingFlags.NonPublic,
            null,
            new[] { typeof(SqlErrorCollection), typeof(string) },
            null);
        Assert.NotNull(create);
        return (SqlException)create!.Invoke(null, new object[] { collection, "11.0.0" })!;
    }

    private static MySqlException CreateMySqlException(int number, string message)
    {
        var code = number == 1054 ? MySqlErrorCode.BadFieldError : (MySqlErrorCode)number;
        var ctor = typeof(MySqlException).GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            new[] { typeof(MySqlErrorCode), typeof(string) },
            null);
        Assert.NotNull(ctor);
        return (MySqlException)ctor!.Invoke(new object[] { code, message });
    }
}

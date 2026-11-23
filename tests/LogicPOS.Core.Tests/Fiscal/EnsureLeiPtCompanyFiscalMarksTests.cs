using LogicPOS.Core.BackOffice;
using LogicPOS.Core.Fiscal;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Interceptors;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Core.Tests.Fiscal;

public sealed class EnsureLeiPtCompanyFiscalMarksTests
{
    private static readonly Guid RegimeId = Guid.Parse("c3d4e5f6-a7b8-4901-c234-56789abcde01");
    private static readonly Guid AmbulanteId = Guid.Parse("d4e5f6a7-b8c9-4012-d345-6789abcdef12");

    [Fact]
    public async Task Single_call_inserts_both_marks()
    {
        await using var db = await CreateDatabaseAsync();
        await BackOfficeListingService.EnsureLeiPtCompanyFiscalMarksAsync(db, CancellationToken.None);

        var tokens = await db.PreferenceParameters
            .Where(item => item.Token == CompanyFiscalMarkTokens.RegimeArtigo53
                           || item.Token == CompanyFiscalMarkTokens.VendedorAmbulante)
            .Select(item => item.Token)
            .ToListAsync();
        Assert.Equal(2, tokens.Count);
        Assert.Contains(CompanyFiscalMarkTokens.RegimeArtigo53, tokens);
        Assert.Contains(CompanyFiscalMarkTokens.VendedorAmbulante, tokens);
    }

    [Fact]
    public async Task Second_sequential_call_is_noop()
    {
        await using var db = await CreateDatabaseAsync();
        await BackOfficeListingService.EnsureLeiPtCompanyFiscalMarksAsync(db, CancellationToken.None);
        await BackOfficeListingService.EnsureLeiPtCompanyFiscalMarksAsync(db, CancellationToken.None);

        Assert.Equal(2, await db.PreferenceParameters.CountAsync(item =>
            item.Token == CompanyFiscalMarkTokens.RegimeArtigo53
            || item.Token == CompanyFiscalMarkTokens.VendedorAmbulante));
    }

    [Fact]
    public async Task Existing_rows_are_left_alone()
    {
        await using var db = await CreateDatabaseAsync();
        db.PreferenceParameters.Add(CreateMark(RegimeId, CompanyFiscalMarkTokens.RegimeArtigo53, 112));
        db.PreferenceParameters.Add(CreateMark(AmbulanteId, CompanyFiscalMarkTokens.VendedorAmbulante, 113));
        await db.SaveChangesAsync();

        await BackOfficeListingService.EnsureLeiPtCompanyFiscalMarksAsync(db, CancellationToken.None);

        Assert.Equal(2, await db.PreferenceParameters.CountAsync());
        Assert.Equal(
            "kept",
            await db.PreferenceParameters
                .Where(item => item.Id == RegimeId)
                .Select(item => item.Value)
                .SingleAsync());
    }

    [Fact]
    public async Task Concurrent_calls_do_not_throw_and_leave_one_row_per_token()
    {
        var path = Path.Combine(Path.GetTempPath(), $"logicpos-lei-pt-{Guid.NewGuid():N}.db");
        try
        {
            await using (var bootstrap = await CreateDatabaseAsync(path))
            {
                // schema only
            }

            async Task InvokeAsync()
            {
                await using var db = OpenDatabase(path);
                await BackOfficeListingService.EnsureLeiPtCompanyFiscalMarksAsync(db, CancellationToken.None);
            }

            await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => InvokeAsync()));

            await using var verify = OpenDatabase(path);
            var rows = await verify.PreferenceParameters
                .Where(item => item.Token == CompanyFiscalMarkTokens.RegimeArtigo53
                               || item.Token == CompanyFiscalMarkTokens.VendedorAmbulante)
                .ToListAsync();
            Assert.Equal(2, rows.Count);
            Assert.Contains(rows, item => item.Id == RegimeId && item.Token == CompanyFiscalMarkTokens.RegimeArtigo53);
            Assert.Contains(rows, item => item.Id == AmbulanteId && item.Token == CompanyFiscalMarkTokens.VendedorAmbulante);
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    private static PreferenceParameter CreateMark(Guid id, string token, uint order) => new()
    {
        Id = id,
        Token = token,
        Value = "kept",
        Required = false,
        RegEx = "RegexBoolean",
        ResourceString = token,
        FormType = 1,
        FormPageNo = 1,
        InputType = LogicPOS.Domain.Enums.PreferenceParameterInputType.CheckButton,
        Order = order,
        Code = order.ToString(),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    private static async Task<LogicPOSDbContext> CreateDatabaseAsync(string? path = null)
    {
        path ??= Path.Combine(Path.GetTempPath(), $"logicpos-lei-pt-{Guid.NewGuid():N}.db");
        var db = OpenDatabase(path);
        await db.Database.MigrateAsync();
        return db;
    }

    private static LogicPOSDbContext OpenDatabase(string path)
    {
        var options = new DbContextOptionsBuilder<LogicPOSDbContext>()
            .UseSqlite($"Data Source={path}", sqlite =>
                sqlite.MigrationsAssembly("LogicPOS.Persistence.Migrators.SQLite"))
            .Options;
        return new LogicPOSDbContext(options, new AuditingInterceptor(new NullAuditing()));
    }

    private sealed class NullAuditing : IAuditingInformationService
    {
        public Guid? GetUserId() => Guid.Empty;

        public Guid? GetTerminalId() => Guid.Empty;

        public void SetUser(Guid? userId)
        {
        }

        public void SetTerminal(Guid? terminalId)
        {
        }
    }
}

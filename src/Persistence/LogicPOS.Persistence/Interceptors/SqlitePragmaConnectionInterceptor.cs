using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LogicPOS.Persistence.Interceptors;

/// <summary>
///     Reforça tolerância a concorrência no ficheiro SQLite (timeout em locks; WAL).
/// </summary>
public sealed class SqlitePragmaConnectionInterceptor : DbConnectionInterceptor
{
    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData) =>
        ApplyPragmas(connection);

    public override async Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default) =>
        await ApplyPragmasAsync(connection, cancellationToken).ConfigureAwait(false);

    private const string Pragmas = "PRAGMA busy_timeout=10000; PRAGMA journal_mode=WAL;";

    private static void ApplyPragmas(DbConnection connection)
    {
        if (connection is not SqliteConnection sqlite)
        {
            return;
        }

        using var cmd = sqlite.CreateCommand();
        cmd.CommandText = Pragmas;
        cmd.ExecuteNonQuery();
    }

    private static async Task ApplyPragmasAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        if (connection is not SqliteConnection sqlite)
        {
            return;
        }

        await using var cmd = sqlite.CreateCommand();
        cmd.CommandText = Pragmas;
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }
}

using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.Persistence.Migrations;

[DbContext(typeof(LogicPOSDbContext))]
[Migration("20261125120000_FixClearAtProductionCredentialsR4")]
public class FixClearAtProductionCredentialsR4 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Clearing runs in C# (SHA-256 compare) via DataFixes.FixClearAtProductionCredentialsR4.Apply —
        // same path for all providers; SQLite has no native SHA. Marker keeps history.
        migrationBuilder.Sql(LogicPOS.Persistence.DataFixes.FixClearAtProductionCredentialsR4.HistoryMarkerSql);
    }

    /// <summary>
    /// Credential-clear migration only: Down is intentionally empty so a rollback does not
    /// restore former AT production seed Values on customer databases.
    /// </summary>
    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}

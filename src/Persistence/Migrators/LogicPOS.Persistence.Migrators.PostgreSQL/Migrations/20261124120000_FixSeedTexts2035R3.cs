using LogicPOS.Persistence.DataFixes;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.Persistence.Migrations;

[DbContext(typeof(LogicPOSDbContext))]
[Migration("20261124120000_FixSeedTexts2035R3")]
public class FixSeedTexts2035R3 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        foreach (var sql in FixSeedTexts2035R3Statements.ForPostgreSql())
        {
            migrationBuilder.Sql(sql);
        }
    }

    /// <summary>
    /// Text-fix migration only: Down is intentionally empty so a rollback does not restore
    /// incorrect Portuguese seed values on customer databases.
    /// </summary>
    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}

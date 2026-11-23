using LogicPOS.Persistence.DataFixes;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.Persistence.Migrators.PostgreSQL.Migrations;

[DbContext(typeof(LogicPOSDbContext))]
[Migration("20261123120000_FixSeedTexts2035")]
public class FixSeedTexts2035 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        foreach (var sql in FixSeedTexts2035Statements.ForPostgreSql())
        {
            migrationBuilder.Sql(sql);
        }
    }

    /// <summary>
    /// Data correction only: Down is intentionally empty so a rollback does not restore
    /// incorrect Portuguese/demo seed values on customer databases.
    /// </summary>
    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}

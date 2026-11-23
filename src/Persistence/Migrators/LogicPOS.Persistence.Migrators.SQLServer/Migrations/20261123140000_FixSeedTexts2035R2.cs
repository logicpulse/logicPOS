using LogicPOS.Persistence.DataFixes;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.Persistence.Migrations;

[DbContext(typeof(LogicPOSDbContext))]
[Migration("20261123140000_FixSeedTexts2035R2")]
public class FixSeedTexts2035R2 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        foreach (var sql in FixSeedTexts2035R2Statements.ForSqlServer())
        {
            migrationBuilder.Sql(sql);
        }
    }

    /// <summary>
    /// Text-fix migration: Down is intentionally empty so a rollback does not restore
    /// incorrect Portuguese seed values. Up also soft-deletes orphan duplicate Final Consumer
    /// rows (non-text); that cleanup is not reversed here.
    /// </summary>
    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}

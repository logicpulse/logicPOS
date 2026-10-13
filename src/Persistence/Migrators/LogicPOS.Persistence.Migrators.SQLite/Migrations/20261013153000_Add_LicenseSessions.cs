using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using LogicPOS.Persistence.Database;

#nullable disable

namespace LogicPOS.Persistence.Migrations;

[DbContext(typeof(LogicPOSDbContext))]
[Migration("20261013153000_Add_LicenseSessions")]
public class Add_LicenseSessions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "LicenseSessions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                ClientKind = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                SessionKey = table.Column<Guid>(type: "TEXT", nullable: false),
                LastHeartbeatAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                TerminalId = table.Column<Guid>(type: "TEXT", nullable: true),
                Notes = table.Column<string>(type: "TEXT", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                CreatedBy = table.Column<Guid>(type: "TEXT", nullable: false),
                CreatedWhere = table.Column<Guid>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: false),
                UpdatedWhere = table.Column<Guid>(type: "TEXT", nullable: false),
                IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LicenseSessions", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_LicenseSessions_LastHeartbeatAt",
            table: "LicenseSessions",
            column: "LastHeartbeatAt");

        migrationBuilder.CreateIndex(
            name: "IX_LicenseSessions_SessionKey",
            table: "LicenseSessions",
            column: "SessionKey",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_LicenseSessions_UserId",
            table: "LicenseSessions",
            column: "UserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "LicenseSessions");
    }
}
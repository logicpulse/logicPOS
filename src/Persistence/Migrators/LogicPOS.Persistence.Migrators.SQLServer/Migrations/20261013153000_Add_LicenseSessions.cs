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
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ClientKind = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                SessionKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                LastHeartbeatAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                TerminalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
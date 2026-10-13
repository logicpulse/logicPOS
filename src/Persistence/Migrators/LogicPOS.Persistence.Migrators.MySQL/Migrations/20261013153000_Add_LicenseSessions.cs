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
                Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                ClientKind = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                SessionKey = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                LastHeartbeatAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                TerminalId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                Notes = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                CreatedWhere = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                UpdatedWhere = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LicenseSessions", x => x.Id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

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
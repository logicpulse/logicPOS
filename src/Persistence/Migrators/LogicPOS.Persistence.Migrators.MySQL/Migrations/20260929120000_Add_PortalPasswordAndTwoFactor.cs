using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using LogicPOS.Persistence.Database;

#nullable disable

namespace LogicPOS.Persistence.Migrations;

[DbContext(typeof(LogicPOSDbContext))]
[Migration("20260929120000_Add_PortalPasswordAndTwoFactor")]
public class Add_PortalPasswordAndTwoFactor : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "Need2FA",
            table: "Users",
            type: "tinyint(1)",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateTable(
            name: "UserVerificationCodes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                CodeHash = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                UsedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
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
                table.PrimaryKey("PK_UserVerificationCodes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "SmsOutboundMessages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                UserId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                Destination = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Purpose = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
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
                table.PrimaryKey("PK_SmsOutboundMessages", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_UserVerificationCodes_UserId",
            table: "UserVerificationCodes",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_SmsOutboundMessages_UserId",
            table: "SmsOutboundMessages",
            column: "UserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "UserVerificationCodes");
        migrationBuilder.DropTable(name: "SmsOutboundMessages");
        migrationBuilder.DropColumn(name: "Need2FA", table: "Users");
    }
}

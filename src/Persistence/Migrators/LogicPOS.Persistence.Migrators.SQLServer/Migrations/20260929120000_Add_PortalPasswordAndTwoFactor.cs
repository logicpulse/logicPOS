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
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateTable(
            name: "UserVerificationCodes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CodeHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                table.PrimaryKey("PK_UserVerificationCodes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "SmsOutboundMessages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Destination = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
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

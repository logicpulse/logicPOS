using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Rename_DocumentPrintings_To_DocumentPrints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SystemAudits_SystemAuditTypes_AuditTypeId",
                table: "SystemAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_SystemNotifications_SystemNotificationTypes_NotificationType~",
                table: "SystemNotifications");

            migrationBuilder.DropTable(
                name: "DocumentPrintings");

            migrationBuilder.RenameColumn(
                name: "NotificationTypeId",
                table: "SystemNotifications",
                newName: "TypeId");
            
            migrationBuilder.AlterColumn<Guid>(
                name: "TypeId",
                table: "SystemNotifications",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)");

            migrationBuilder.RenameIndex(
                name: "IX_SystemNotifications_NotificationTypeId",
                table: "SystemNotifications",
                newName: "IX_SystemNotifications_TypeId");

            migrationBuilder.RenameColumn(
                name: "AuditTypeId",
                table: "SystemAudits",
                newName: "TypeId");
            
            migrationBuilder.AlterColumn<Guid>(
                name: "TypeId",
                table: "SystemAudits",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)");

            migrationBuilder.RenameIndex(
                name: "IX_SystemAudits_AuditTypeId",
                table: "SystemAudits",
                newName: "IX_SystemAudits_TypeId");

            migrationBuilder.CreateTable(
                name: "DocumentPrints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Designation = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CopyNames = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PrintCopies = table.Column<int>(type: "int", nullable: false),
                    PrintMotive = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecondPrint = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DocumentId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ReceiptId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
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
                    table.PrimaryKey("PK_DocumentPrints", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_SystemAudits_SystemAuditTypes_TypeId",
                table: "SystemAudits",
                column: "TypeId",
                principalTable: "SystemAuditTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SystemNotifications_SystemNotificationTypes_TypeId",
                table: "SystemNotifications",
                column: "TypeId",
                principalTable: "SystemNotificationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SystemAudits_SystemAuditTypes_TypeId",
                table: "SystemAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_SystemNotifications_SystemNotificationTypes_TypeId",
                table: "SystemNotifications");

            migrationBuilder.DropTable(
                name: "DocumentPrints");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "SystemNotifications",
                newName: "NotificationTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_SystemNotifications_TypeId",
                table: "SystemNotifications",
                newName: "IX_SystemNotifications_NotificationTypeId");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "SystemAudits",
                newName: "AuditTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_SystemAudits_TypeId",
                table: "SystemAudits",
                newName: "IX_SystemAudits_AuditTypeId");

            migrationBuilder.CreateTable(
                name: "DocumentPrintings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CopyNames = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedWhere = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Designation = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PrintCopies = table.Column<int>(type: "int", nullable: false),
                    PrintMotive = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReceiptId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    SecondPrint = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UpdatedWhere = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentPrintings", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_SystemAudits_SystemAuditTypes_AuditTypeId",
                table: "SystemAudits",
                column: "AuditTypeId",
                principalTable: "SystemAuditTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SystemNotifications_SystemNotificationTypes_NotificationType~",
                table: "SystemNotifications",
                column: "NotificationTypeId",
                principalTable: "SystemNotificationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

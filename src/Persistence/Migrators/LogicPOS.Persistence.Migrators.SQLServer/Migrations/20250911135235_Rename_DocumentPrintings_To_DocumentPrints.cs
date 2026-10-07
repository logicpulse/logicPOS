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
                name: "FK_SystemNotifications_SystemNotificationTypes_NotificationTypeId",
                table: "SystemNotifications");

            migrationBuilder.DropTable(
                name: "DocumentPrintings");

            migrationBuilder.RenameColumn(
                name: "NotificationTypeId",
                table: "SystemNotifications",
                newName: "TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_SystemNotifications_NotificationTypeId",
                table: "SystemNotifications",
                newName: "IX_SystemNotifications_TypeId");

            migrationBuilder.RenameColumn(
                name: "AuditTypeId",
                table: "SystemAudits",
                newName: "TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_SystemAudits_AuditTypeId",
                table: "SystemAudits",
                newName: "IX_SystemAudits_TypeId");

            migrationBuilder.CreateTable(
                name: "DocumentPrints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CopyNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrintCopies = table.Column<int>(type: "int", nullable: false),
                    PrintMotive = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondPrint = table.Column<bool>(type: "bit", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReceiptId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_DocumentPrints", x => x.Id);
                });

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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CopyNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrintCopies = table.Column<int>(type: "int", nullable: false),
                    PrintMotive = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SecondPrint = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentPrintings", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_SystemAudits_SystemAuditTypes_AuditTypeId",
                table: "SystemAudits",
                column: "AuditTypeId",
                principalTable: "SystemAuditTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SystemNotifications_SystemNotificationTypes_NotificationTypeId",
                table: "SystemNotifications",
                column: "NotificationTypeId",
                principalTable: "SystemNotificationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

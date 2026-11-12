using System;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.Persistence.Migrators.PostgreSQL.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(LogicPOSDbContext))]
    [Migration("20261112160000_Add_AgtDocumentInfo_SyncFields")]
    public partial class Add_AgtDocumentInfo_SyncFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AddSyncColumns(migrationBuilder, "Documents", "timestamp without time zone", "integer", "boolean");
            AddSyncColumns(migrationBuilder, "Receipts", "timestamp without time zone", "integer", "boolean");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            DropSyncColumns(migrationBuilder, "Documents");
            DropSyncColumns(migrationBuilder, "Receipts");
        }

        private static void AddSyncColumns(
            MigrationBuilder migrationBuilder,
            string table,
            string dateType,
            string intType,
            string boolType)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Agt_LastSyncAt",
                table: table,
                type: dateType,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Agt_SubmissionAttempts",
                table: table,
                type: intType,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Agt_SubmissionPending",
                table: table,
                type: boolType,
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Agt_ValidationSyncPending",
                table: table,
                type: boolType,
                nullable: false,
                defaultValue: false);
        }

        private static void DropSyncColumns(MigrationBuilder migrationBuilder, string table)
        {
            migrationBuilder.DropColumn(name: "Agt_LastSyncAt", table: table);
            migrationBuilder.DropColumn(name: "Agt_SubmissionAttempts", table: table);
            migrationBuilder.DropColumn(name: "Agt_SubmissionPending", table: table);
            migrationBuilder.DropColumn(name: "Agt_ValidationSyncPending", table: table);
        }
    }
}

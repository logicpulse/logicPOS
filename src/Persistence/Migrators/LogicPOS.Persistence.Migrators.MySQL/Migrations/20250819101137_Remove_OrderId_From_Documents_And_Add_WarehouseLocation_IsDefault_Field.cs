using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Remove_OrderId_From_Documents_And_Add_WarehouseLocation_IsDefault_Field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Orders_OrderId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_OrderId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Documents");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "WarehouseLocations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "WarehouseLocations");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "Documents",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_OrderId",
                table: "Documents",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Orders_OrderId",
                table: "Documents",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_OrderDocuments_Table_And_Adjust_Quantity_Type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "WarehouseArticles",
                type: "decimal(20,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "StockMovements",
                type: "decimal(20,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "OrderDetails",
                type: "decimal(20,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "DocumentDetails",
                type: "decimal(20,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinimumStock",
                table: "Articles",
                type: "decimal(20,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DefaultQuantity",
                table: "Articles",
                type: "decimal(20,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "ArticleCompositions",
                type: "decimal(20,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,5)");

            migrationBuilder.CreateTable(
                name: "OrderDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DocumentId = table.Column<Guid>(type: "TEXT", nullable: false),
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
                    table.PrimaryKey("PK_OrderDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDocuments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDocuments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDocuments_DocumentId",
                table: "OrderDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDocuments_OrderId",
                table: "OrderDocuments",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDocuments");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "WarehouseArticles",
                type: "decimal(10,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "StockMovements",
                type: "decimal(10,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "OrderDetails",
                type: "decimal(10,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "DocumentDetails",
                type: "decimal(10,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinimumStock",
                table: "Articles",
                type: "decimal(10,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DefaultQuantity",
                table: "Articles",
                type: "decimal(10,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "ArticleCompositions",
                type: "decimal(10,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,5)");
        }
    }
}

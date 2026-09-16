using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.ApiServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStocksAndWarehouses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiStockMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SupplierId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DocumentNumber = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ExternalDocument = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiStockMovements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiWarehouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<uint>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiWarehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiStockMovementItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    StockMovementId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ArticleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", nullable: false),
                    SerialNumber = table.Column<string>(type: "TEXT", nullable: true),
                    WarehouseLocationId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiStockMovementItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiStockMovementItems_ApiArticles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "ApiArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApiStockMovementItems_ApiStockMovements_StockMovementId",
                        column: x => x.StockMovementId,
                        principalTable: "ApiStockMovements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiWarehouseLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiWarehouseLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiWarehouseLocations_ApiWarehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "ApiWarehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApiWarehouseArticles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ArticleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SerialNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    WarehouseLocationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiWarehouseArticles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiWarehouseArticles_ApiArticles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "ApiArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApiWarehouseArticles_ApiWarehouseLocations_WarehouseLocationId",
                        column: x => x.WarehouseLocationId,
                        principalTable: "ApiWarehouseLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiStockMovementItems_ArticleId",
                table: "ApiStockMovementItems",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiStockMovementItems_StockMovementId",
                table: "ApiStockMovementItems",
                column: "StockMovementId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiWarehouseArticles_ArticleId",
                table: "ApiWarehouseArticles",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiWarehouseArticles_WarehouseLocationId",
                table: "ApiWarehouseArticles",
                column: "WarehouseLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiWarehouseLocations_WarehouseId",
                table: "ApiWarehouseLocations",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiStockMovementItems");

            migrationBuilder.DropTable(
                name: "ApiWarehouseArticles");

            migrationBuilder.DropTable(
                name: "ApiStockMovements");

            migrationBuilder.DropTable(
                name: "ApiWarehouseLocations");

            migrationBuilder.DropTable(
                name: "ApiWarehouses");
        }
    }
}

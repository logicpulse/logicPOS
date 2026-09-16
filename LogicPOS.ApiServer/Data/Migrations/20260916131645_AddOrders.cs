using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.ApiServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TableId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    DeleteReason = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiOrders_ApiTables_TableId",
                        column: x => x.TableId,
                        principalTable: "ApiTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApiOrderTickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TicketNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    SplittersNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiOrderTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiOrderTickets_ApiOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "ApiOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiOrderDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TicketId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ArticleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Discount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Vat = table.Column<decimal>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiOrderDetails_ApiArticles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "ApiArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApiOrderDetails_ApiOrderTickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "ApiOrderTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiOrderDetails_ArticleId",
                table: "ApiOrderDetails",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiOrderDetails_TicketId",
                table: "ApiOrderDetails",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiOrders_TableId",
                table: "ApiOrders",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiOrderTickets_OrderId",
                table: "ApiOrderTickets",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiOrderDetails");

            migrationBuilder.DropTable(
                name: "ApiOrderTickets");

            migrationBuilder.DropTable(
                name: "ApiOrders");
        }
    }
}

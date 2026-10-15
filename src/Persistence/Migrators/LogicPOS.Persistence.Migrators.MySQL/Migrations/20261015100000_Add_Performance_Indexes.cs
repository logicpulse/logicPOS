using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using LogicPOS.Persistence.Database;

#nullable disable

namespace LogicPOS.Persistence.Migrations;

[DbContext(typeof(LogicPOSDbContext))]
[Migration("20261015100000_Add_Performance_Indexes")]
public class Add_Performance_Indexes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_Articles_IsDeleted_Order",
            table: "Articles",
            columns: new[] { "IsDeleted", "Order" });

        migrationBuilder.CreateIndex(
            name: "IX_Articles_Favorite",
            table: "Articles",
            column: "Favorite");

        migrationBuilder.CreateIndex(
            name: "IX_ArticleFamilies_IsDeleted_Order",
            table: "ArticleFamilies",
            columns: new[] { "IsDeleted", "Order" });

        migrationBuilder.CreateIndex(
            name: "IX_ArticleSubfamilies_IsDeleted_Order",
            table: "ArticleSubfamilies",
            columns: new[] { "IsDeleted", "Order" });

        migrationBuilder.CreateIndex(
            name: "IX_Documents_CreatedAt",
            table: "Documents",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_Documents_IsDeleted_CreatedAt",
            table: "Documents",
            columns: new[] { "IsDeleted", "CreatedAt" });

        migrationBuilder.CreateIndex(
            name: "IX_Customers_IsDeleted",
            table: "Customers",
            column: "IsDeleted");

        migrationBuilder.CreateIndex(
            name: "IX_Customers_FiscalNumber",
            table: "Customers",
            column: "FiscalNumber");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_Articles_IsDeleted_Order", table: "Articles");
        migrationBuilder.DropIndex(name: "IX_Articles_Favorite", table: "Articles");
        migrationBuilder.DropIndex(name: "IX_ArticleFamilies_IsDeleted_Order", table: "ArticleFamilies");
        migrationBuilder.DropIndex(name: "IX_ArticleSubfamilies_IsDeleted_Order", table: "ArticleSubfamilies");
        migrationBuilder.DropIndex(name: "IX_Documents_CreatedAt", table: "Documents");
        migrationBuilder.DropIndex(name: "IX_Documents_IsDeleted_CreatedAt", table: "Documents");
        migrationBuilder.DropIndex(name: "IX_Customers_IsDeleted", table: "Customers");
        migrationBuilder.DropIndex(name: "IX_Customers_FiscalNumber", table: "Customers");
    }
}

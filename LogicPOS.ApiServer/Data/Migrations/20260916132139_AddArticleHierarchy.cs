using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.ApiServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiArticleChildren",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParentArticleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChildArticleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiArticleChildren", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiArticleChildren_ApiArticles_ChildArticleId",
                        column: x => x.ChildArticleId,
                        principalTable: "ApiArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApiArticleChildren_ApiArticles_ParentArticleId",
                        column: x => x.ParentArticleId,
                        principalTable: "ApiArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiArticleChildren_ChildArticleId",
                table: "ApiArticleChildren",
                column: "ChildArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiArticleChildren_ParentArticleId_ChildArticleId",
                table: "ApiArticleChildren",
                columns: new[] { "ParentArticleId", "ChildArticleId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiArticleChildren");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Field_TerminalId_To_DocumentSeries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TerminalId",
                table: "DocumentSeries",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSeries_TerminalId",
                table: "DocumentSeries",
                column: "TerminalId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentSeries_Terminals_TerminalId",
                table: "DocumentSeries",
                column: "TerminalId",
                principalTable: "Terminals",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentSeries_Terminals_TerminalId",
                table: "DocumentSeries");

            migrationBuilder.DropIndex(
                name: "IX_DocumentSeries_TerminalId",
                table: "DocumentSeries");

            migrationBuilder.DropColumn(
                name: "TerminalId",
                table: "DocumentSeries");
        }
    }
}

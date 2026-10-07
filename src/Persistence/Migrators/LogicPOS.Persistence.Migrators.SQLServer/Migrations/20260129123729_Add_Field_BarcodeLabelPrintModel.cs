using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Field_BarcodeLabelPrintModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TerminalDocumentSeries");

            migrationBuilder.AddColumn<string>(
                name: "BarcodeLabelPrintModel",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarcodeLabelPrintModel",
                table: "Articles");

            migrationBuilder.CreateTable(
                name: "TerminalDocumentSeries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TerminalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminalDocumentSeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TerminalDocumentSeries_DocumentSeries_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "DocumentSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TerminalDocumentSeries_Terminals_TerminalId",
                        column: x => x.TerminalId,
                        principalTable: "Terminals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TerminalDocumentSeries_SeriesId",
                table: "TerminalDocumentSeries",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_TerminalDocumentSeries_TerminalId",
                table: "TerminalDocumentSeries",
                column: "TerminalId");
        }
    }
}

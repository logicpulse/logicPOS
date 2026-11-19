using System;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.Persistence.Migrations;

[DbContext(typeof(LogicPOSDbContext))]
[Migration("20261119160000_Add_PostalLocalities")]
public class Add_PostalLocalities : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "PostalLocalities",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                CountryId = table.Column<Guid>(type: "TEXT", nullable: false),
                ZipCode = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                Locality = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                City = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
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
                table.PrimaryKey("PK_PostalLocalities", x => x.Id);
                table.ForeignKey(
                    name: "FK_PostalLocalities_Countries_CountryId",
                    column: x => x.CountryId,
                    principalTable: "Countries",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_PostalLocalities_CountryId_ZipCode",
            table: "PostalLocalities",
            columns: new[] { "CountryId", "ZipCode" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "PostalLocalities");
    }
}

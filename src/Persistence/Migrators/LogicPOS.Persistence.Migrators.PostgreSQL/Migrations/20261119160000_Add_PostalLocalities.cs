using System;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.Persistence.Migrators.PostgreSQL.Migrations;

[DbContext(typeof(LogicPOSDbContext))]
[Migration("20261119160000_Add_PostalLocalities")]
public partial class Add_PostalLocalities : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "PostalLocalities",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CountryId = table.Column<Guid>(type: "uuid", nullable: false),
                ZipCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Locality = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                City = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                Notes = table.Column<string>(type: "text", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedWhere = table.Column<Guid>(type: "uuid", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                UpdatedWhere = table.Column<Guid>(type: "uuid", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
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

using System;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.Persistence.Migrators.PostgreSQL.Migrations;

[DbContext(typeof(LogicPOSDbContext))]
[Migration("20261119180000_Add_PostalLocalityStreet")]
public class Add_PostalLocalityStreet : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Street",
            table: "PostalLocalities",
            type: "character varying(160)",
            maxLength: 160,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Street", table: "PostalLocalities");
    }
}

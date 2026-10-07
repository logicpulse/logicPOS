using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using LogicPOS.Persistence.Database;

#nullable disable

namespace LogicPOS.Persistence.Migrations;

[DbContext(typeof(LogicPOSDbContext))]
[Migration("20260921120000_Add_Field_IsTicketing_To_Articles")]
public class Add_Field_IsTicketing_To_Articles : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsTicketing",
            table: "Articles",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsTicketing",
            table: "Articles");
    }
}

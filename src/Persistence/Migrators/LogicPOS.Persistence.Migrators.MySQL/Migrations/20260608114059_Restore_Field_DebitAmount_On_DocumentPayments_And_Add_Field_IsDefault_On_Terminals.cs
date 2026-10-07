using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Restore_Field_DebitAmount_On_DocumentPayments_And_Add_Field_IsDefault_On_Terminals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Terminals",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "DebitAmount",
                table: "Payments",
                type: "decimal(18,6)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Terminals");

            migrationBuilder.DropColumn(
                name: "DebitAmount",
                table: "Payments");
        }
    }
}

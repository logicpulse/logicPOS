using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Field_RejectedDocumentNumber_To_AgtDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectedDocumentNumber",
                table: "AgtDocuments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectedDocumentNumber",
                table: "AgtDocuments");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Field_IsSdrPackaging_To_Articles_And_Remove_AgtDocuments_And_Add_Field_CardMode_To_Customers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgtDocuments");

            migrationBuilder.AddColumn<int>(
                name: "CardMode",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsSdrPackaging",
                table: "Articles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardMode",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsSdrPackaging",
                table: "Articles");

            migrationBuilder.CreateTable(
                name: "AgtDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HttpStatusCode = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RejectedDocumentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmissionErrorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmissionErrorDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmissionUuid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValidationErrors = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidationResultCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgtDocuments", x => x.Id);
                });
        }
    }
}

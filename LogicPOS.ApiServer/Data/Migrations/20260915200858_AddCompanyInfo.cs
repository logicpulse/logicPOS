using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.ApiServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiCompanyInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    BusinessName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    CommercialName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    LogoPng = table.Column<string>(type: "TEXT", nullable: false),
                    LogoBmp = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    PostalCode = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    CountryCode2 = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    MobilePhone = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Website = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    FiscalNumber = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    StockCapital = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    DocumentFinalLine1 = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    DocumentFinalLine2 = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    TaxEntity = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Fax = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    TicketFinalLine1 = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    TicketFinalLine2 = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    CurrencyCode = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    AgtLogo = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiCompanyInfos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiCompanyInfos");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.ApiServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLicensingAndTerminals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiLicenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsLicensed = table.Column<bool>(type: "INTEGER", nullable: false),
                    Version = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    HardwareId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Company = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Nif = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Reseller = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    StocksModule = table.Column<bool>(type: "INTEGER", nullable: false),
                    AgtFeModule = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllUpdateExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AllNumberOfDevices = table.Column<int>(type: "INTEGER", nullable: true),
                    HasExpired = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsValid = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiLicenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiTerminals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<uint>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    HardwareId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    TimerInterval = table.Column<uint>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiTerminals", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiTerminals_HardwareId",
                table: "ApiTerminals",
                column: "HardwareId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiLicenses");

            migrationBuilder.DropTable(
                name: "ApiTerminals");
        }
    }
}

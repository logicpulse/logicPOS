using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.ApiServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentsAndReceipts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiDocumentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<uint>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Acronym = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    PrintCopies = table.Column<int>(type: "INTEGER", nullable: false),
                    PrintRequestMotive = table.Column<bool>(type: "INTEGER", nullable: false),
                    PrintRequestConfirmation = table.Column<bool>(type: "INTEGER", nullable: false),
                    PrintOpenDrawer = table.Column<bool>(type: "INTEGER", nullable: false),
                    SaftDocumentType = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiDocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiFiscalYears",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<uint>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Acronym = table.Column<string>(type: "TEXT", nullable: true),
                    SeriesForEachTerminal = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsClosed = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiFiscalYears", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiPaymentMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<uint>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: true),
                    ResourceString = table.Column<string>(type: "TEXT", nullable: true),
                    ButtonIcon = table.Column<string>(type: "TEXT", nullable: true),
                    Acronym = table.Column<string>(type: "TEXT", nullable: true),
                    AllowPayback = table.Column<string>(type: "TEXT", nullable: true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiPaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiDocumentSeries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    NextNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    NumberRangeBegin = table.Column<int>(type: "INTEGER", nullable: false),
                    NumberRangeEnd = table.Column<int>(type: "INTEGER", nullable: false),
                    Acronym = table.Column<string>(type: "TEXT", nullable: true),
                    DocumentTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FiscalYearId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AtValidationCode = table.Column<string>(type: "TEXT", nullable: true),
                    TerminalId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiDocumentSeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiDocumentSeries_ApiDocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "ApiDocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApiDocumentSeries_ApiFiscalYears_FiscalYearId",
                        column: x => x.FiscalYearId,
                        principalTable: "ApiFiscalYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApiDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    DocumentSeriesId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Number = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    OrderId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerFiscalNumber = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerAddress = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerLocality = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerZipCode = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerCity = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerEmail = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerPhone = table.Column<string>(type: "TEXT", nullable: true),
                    Discount = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalNet = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalDiscount = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalTax = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalFinal = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalPaid = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CancelReason = table.Column<string>(type: "TEXT", nullable: true),
                    RelatedDocumentNumbers = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiDocuments_ApiDocumentSeries_DocumentSeriesId",
                        column: x => x.DocumentSeriesId,
                        principalTable: "ApiDocumentSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApiDocumentDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DocumentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<uint>(type: "INTEGER", nullable: false),
                    ArticleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", nullable: true),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Discount = table.Column<decimal>(type: "TEXT", nullable: false),
                    VatRateId = table.Column<Guid>(type: "TEXT", nullable: true),
                    VatDesignation = table.Column<string>(type: "TEXT", nullable: true),
                    VatCode = table.Column<string>(type: "TEXT", nullable: true),
                    VatPercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    VatExemptionReasonId = table.Column<Guid>(type: "TEXT", nullable: true),
                    VatExemptionReason = table.Column<string>(type: "TEXT", nullable: true),
                    TotalNet = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalDiscount = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalTax = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalFinal = table.Column<decimal>(type: "TEXT", nullable: false),
                    SerialNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Warehouse = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiDocumentDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiDocumentDetails_ApiArticles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "ApiArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApiDocumentDetails_ApiDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "ApiDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiDocumentPaymentMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DocumentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiDocumentPaymentMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiDocumentPaymentMethods_ApiDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "ApiDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApiDocumentPaymentMethods_ApiPaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "ApiPaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiDocumentDetails_ArticleId",
                table: "ApiDocumentDetails",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiDocumentDetails_DocumentId",
                table: "ApiDocumentDetails",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiDocumentPaymentMethods_DocumentId",
                table: "ApiDocumentPaymentMethods",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiDocumentPaymentMethods_PaymentMethodId",
                table: "ApiDocumentPaymentMethods",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiDocuments_DocumentSeriesId",
                table: "ApiDocuments",
                column: "DocumentSeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiDocumentSeries_DocumentTypeId",
                table: "ApiDocumentSeries",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiDocumentSeries_FiscalYearId",
                table: "ApiDocumentSeries",
                column: "FiscalYearId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiDocumentTypes_Acronym",
                table: "ApiDocumentTypes",
                column: "Acronym",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiDocumentDetails");

            migrationBuilder.DropTable(
                name: "ApiDocumentPaymentMethods");

            migrationBuilder.DropTable(
                name: "ApiDocuments");

            migrationBuilder.DropTable(
                name: "ApiPaymentMethods");

            migrationBuilder.DropTable(
                name: "ApiDocumentSeries");

            migrationBuilder.DropTable(
                name: "ApiDocumentTypes");

            migrationBuilder.DropTable(
                name: "ApiFiscalYears");
        }
    }
}

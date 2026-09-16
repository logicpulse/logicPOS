using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.ApiServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddArticlesAndCatalogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiArticleClasses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<uint>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Acronym = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    WorkInStock = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiArticleClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiArticleFamilies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<uint>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    CommissionGroupId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DiscountGroupId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ButtonLabel = table.Column<string>(type: "TEXT", nullable: true),
                    ButtonImage = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiArticleFamilies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiArticleTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<uint>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    HasPrice = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiArticleTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiMeasurementUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<uint>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Acronym = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiMeasurementUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiSizeUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<uint>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiSizeUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiVatRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<uint>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Value = table.Column<decimal>(type: "TEXT", nullable: false),
                    ReasonCode = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    TaxType = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    TaxCode = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    CountryRegion = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiVatRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiArticleSubfamilies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<uint>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    FamilyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CommissionGroupId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DiscountGroupId = table.Column<Guid>(type: "TEXT", nullable: true),
                    VatOnTableId = table.Column<Guid>(type: "TEXT", nullable: true),
                    VatDirectSellingId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ButtonLabel = table.Column<string>(type: "TEXT", nullable: true),
                    ButtonImage = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiArticleSubfamilies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiArticleSubfamilies_ApiArticleFamilies_FamilyId",
                        column: x => x.FamilyId,
                        principalTable: "ApiArticleFamilies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApiArticles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<uint>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    CodeDealer = table.Column<string>(type: "TEXT", nullable: true),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    ClassId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SubfamilyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MeasurementUnitId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SizeUnitId = table.Column<Guid>(type: "TEXT", nullable: false),
                    VatDirectSellingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CommissionGroupId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DiscountGroupId = table.Column<Guid>(type: "TEXT", nullable: true),
                    VatOnTableId = table.Column<Guid>(type: "TEXT", nullable: true),
                    VatExemptionReasonId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ButtonLabel = table.Column<string>(type: "TEXT", nullable: true),
                    ButtonImage = table.Column<string>(type: "TEXT", nullable: true),
                    Price1Value = table.Column<decimal>(type: "TEXT", nullable: false),
                    Price1PromotionValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    Price1UsePromotion = table.Column<bool>(type: "INTEGER", nullable: false),
                    Price2Value = table.Column<decimal>(type: "TEXT", nullable: false),
                    Price2PromotionValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    Price2UsePromotion = table.Column<bool>(type: "INTEGER", nullable: false),
                    Price3Value = table.Column<decimal>(type: "TEXT", nullable: false),
                    Price3PromotionValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    Price3UsePromotion = table.Column<bool>(type: "INTEGER", nullable: false),
                    Price4Value = table.Column<decimal>(type: "TEXT", nullable: false),
                    Price4PromotionValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    Price4UsePromotion = table.Column<bool>(type: "INTEGER", nullable: false),
                    Price5Value = table.Column<decimal>(type: "TEXT", nullable: false),
                    Price5PromotionValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    Price5UsePromotion = table.Column<bool>(type: "INTEGER", nullable: false),
                    PriceWithVat = table.Column<bool>(type: "INTEGER", nullable: false),
                    Discount = table.Column<decimal>(type: "TEXT", nullable: false),
                    DefaultQuantity = table.Column<decimal>(type: "TEXT", nullable: false),
                    MinimumStock = table.Column<decimal>(type: "TEXT", nullable: false),
                    Tare = table.Column<decimal>(type: "TEXT", nullable: false),
                    Weight = table.Column<float>(type: "REAL", nullable: false),
                    Barcode = table.Column<string>(type: "TEXT", nullable: true),
                    PVPVariable = table.Column<bool>(type: "INTEGER", nullable: false),
                    Favorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    UseWeighingBalance = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsComposed = table.Column<bool>(type: "INTEGER", nullable: false),
                    UniqueArticles = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSdrPackaging = table.Column<bool>(type: "INTEGER", nullable: false),
                    BarcodeLabelPrintModel = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiArticles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiArticles_ApiArticleClasses_ClassId",
                        column: x => x.ClassId,
                        principalTable: "ApiArticleClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApiArticles_ApiArticleSubfamilies_SubfamilyId",
                        column: x => x.SubfamilyId,
                        principalTable: "ApiArticleSubfamilies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApiArticles_ApiArticleTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "ApiArticleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApiArticles_ApiMeasurementUnits_MeasurementUnitId",
                        column: x => x.MeasurementUnitId,
                        principalTable: "ApiMeasurementUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApiArticles_ApiSizeUnits_SizeUnitId",
                        column: x => x.SizeUnitId,
                        principalTable: "ApiSizeUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApiArticles_ApiVatRates_VatDirectSellingId",
                        column: x => x.VatDirectSellingId,
                        principalTable: "ApiVatRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiArticles_ClassId",
                table: "ApiArticles",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiArticles_MeasurementUnitId",
                table: "ApiArticles",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiArticles_SizeUnitId",
                table: "ApiArticles",
                column: "SizeUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiArticles_SubfamilyId",
                table: "ApiArticles",
                column: "SubfamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiArticles_TypeId",
                table: "ApiArticles",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiArticles_VatDirectSellingId",
                table: "ApiArticles",
                column: "VatDirectSellingId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiArticleSubfamilies_FamilyId",
                table: "ApiArticleSubfamilies",
                column: "FamilyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiArticles");

            migrationBuilder.DropTable(
                name: "ApiArticleClasses");

            migrationBuilder.DropTable(
                name: "ApiArticleSubfamilies");

            migrationBuilder.DropTable(
                name: "ApiArticleTypes");

            migrationBuilder.DropTable(
                name: "ApiMeasurementUnits");

            migrationBuilder.DropTable(
                name: "ApiSizeUnits");

            migrationBuilder.DropTable(
                name: "ApiVatRates");

            migrationBuilder.DropTable(
                name: "ApiArticleFamilies");
        }
    }
}

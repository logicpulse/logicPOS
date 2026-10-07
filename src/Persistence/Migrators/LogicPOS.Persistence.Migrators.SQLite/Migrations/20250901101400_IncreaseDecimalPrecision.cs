using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseDecimalPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "WorkSessionMovements",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "WarehouseArticles",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "VatRates",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Tickets",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Tables",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "StockMovements",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "StockMovements",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "WithholdingTaxAmount",
                table: "Receipts",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxPayable",
                table: "Receipts",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrencyAmount",
                table: "Receipts",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Receipts",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CreditAmount",
                table: "Payments",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Vat",
                table: "OrderDetails",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalTax",
                table: "OrderDetails",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalFinal",
                table: "OrderDetails",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalDiscount",
                table: "OrderDetails",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "OrderDetails",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "OrderDetails",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "OrderDetails",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "WithholdingTaxAmount",
                table: "Documents",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalTax",
                table: "Documents",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalNet",
                table: "Documents",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalFinal",
                table: "Documents",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalDiscount",
                table: "Documents",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalDelivery",
                table: "Documents",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalChange",
                table: "Documents",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Documents",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "DocumentPaymentMethods",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalTax",
                table: "DocumentDetails",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalNet",
                table: "DocumentDetails",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalFinal",
                table: "DocumentDetails",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalDiscount",
                table: "DocumentDetails",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Tax_Percentage",
                table: "DocumentDetails",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "DocumentDetails",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "DocumentDetails",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "DocumentDetails",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Customers",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CardCredit",
                table: "Customers",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Commissions",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CommissionValue",
                table: "Commissions",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Commission",
                table: "CommissionGroups",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price5_Value",
                table: "Articles",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price5_PromotionValue",
                table: "Articles",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price4_Value",
                table: "Articles",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price4_PromotionValue",
                table: "Articles",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price3_Value",
                table: "Articles",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price3_PromotionValue",
                table: "Articles",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price2_Value",
                table: "Articles",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price2_PromotionValue",
                table: "Articles",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price1_Value",
                table: "Articles",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price1_PromotionValue",
                table: "Articles",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinimumStock",
                table: "Articles",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Articles",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DefaultQuantity",
                table: "Articles",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "ArticleCompositions",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,5)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "WorkSessionMovements",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "WarehouseArticles",
                type: "decimal(20,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "VatRates",
                type: "decimal(5,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Tickets",
                type: "decimal(5,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Tables",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "StockMovements",
                type: "decimal(20,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "StockMovements",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "WithholdingTaxAmount",
                table: "Receipts",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxPayable",
                table: "Receipts",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrencyAmount",
                table: "Receipts",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Receipts",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CreditAmount",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Vat",
                table: "OrderDetails",
                type: "decimal(5,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalTax",
                table: "OrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalFinal",
                table: "OrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalDiscount",
                table: "OrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "OrderDetails",
                type: "decimal(20,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "OrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "OrderDetails",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "WithholdingTaxAmount",
                table: "Documents",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalTax",
                table: "Documents",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalNet",
                table: "Documents",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalFinal",
                table: "Documents",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalDiscount",
                table: "Documents",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalDelivery",
                table: "Documents",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalChange",
                table: "Documents",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Documents",
                type: "decimal(5,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "DocumentPaymentMethods",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalTax",
                table: "DocumentDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalNet",
                table: "DocumentDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalFinal",
                table: "DocumentDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalDiscount",
                table: "DocumentDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Tax_Percentage",
                table: "DocumentDetails",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "DocumentDetails",
                type: "decimal(20,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "DocumentDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "DocumentDetails",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Customers",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CardCredit",
                table: "Customers",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Commissions",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CommissionValue",
                table: "Commissions",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Commission",
                table: "CommissionGroups",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price5_Value",
                table: "Articles",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price5_PromotionValue",
                table: "Articles",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price4_Value",
                table: "Articles",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price4_PromotionValue",
                table: "Articles",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price3_Value",
                table: "Articles",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price3_PromotionValue",
                table: "Articles",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price2_Value",
                table: "Articles",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price2_PromotionValue",
                table: "Articles",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price1_Value",
                table: "Articles",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price1_PromotionValue",
                table: "Articles",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinimumStock",
                table: "Articles",
                type: "decimal(20,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Articles",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DefaultQuantity",
                table: "Articles",
                type: "decimal(20,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "ArticleCompositions",
                type: "decimal(20,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");
        }
    }
}

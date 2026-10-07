using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_AgtDocumentInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Agt_HttpStatusCode",
                table: "Receipts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Agt_RejectedDocumentNumber",
                table: "Receipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Agt_RequestId",
                table: "Receipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Agt_SubmissionDate",
                table: "Receipts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Agt_SubmissionErrorCode",
                table: "Receipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Agt_SubmissionErrorDescription",
                table: "Receipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Agt_SubmissionUuid",
                table: "Receipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Agt_ValidationErrors",
                table: "Receipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Agt_ValidationResultCode",
                table: "Receipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Agt_ValidationStatus",
                table: "Receipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Agt_HttpStatusCode",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Agt_RejectedDocumentNumber",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Agt_RequestId",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Agt_SubmissionDate",
                table: "Documents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Agt_SubmissionErrorCode",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Agt_SubmissionErrorDescription",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Agt_SubmissionUuid",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Agt_ValidationErrors",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Agt_ValidationResultCode",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Agt_ValidationStatus",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Agt_HttpStatusCode",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Agt_RejectedDocumentNumber",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Agt_RequestId",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Agt_SubmissionDate",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Agt_SubmissionErrorCode",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Agt_SubmissionErrorDescription",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Agt_SubmissionUuid",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Agt_ValidationErrors",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Agt_ValidationResultCode",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Agt_ValidationStatus",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Agt_HttpStatusCode",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Agt_RejectedDocumentNumber",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Agt_RequestId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Agt_SubmissionDate",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Agt_SubmissionErrorCode",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Agt_SubmissionErrorDescription",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Agt_SubmissionUuid",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Agt_ValidationErrors",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Agt_ValidationResultCode",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Agt_ValidationStatus",
                table: "Documents");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicPOS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticleClasses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Acronym = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkInStock = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticleTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HasPrice = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommissionGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Commission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capital = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TLD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FiscalNumberRegex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZipCodeRegex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Acronym = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscountGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentPrintings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CopyNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrintCopies = table.Column<int>(type: "int", nullable: false),
                    PrintMotive = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondPrint = table.Column<bool>(type: "bit", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReceiptId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentPrintings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Acronym = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcronymLastSerie = table.Column<int>(type: "int", nullable: true),
                    Credit = table.Column<bool>(type: "bit", nullable: false),
                    CreditDebit = table.Column<int>(type: "int", nullable: true),
                    PrintCopies = table.Column<int>(type: "int", nullable: false),
                    PrintRequestMotive = table.Column<bool>(type: "bit", nullable: false),
                    PrintRequestConfirmation = table.Column<bool>(type: "bit", nullable: false),
                    PrintOpenDrawer = table.Column<bool>(type: "bit", nullable: false),
                    WayBill = table.Column<bool>(type: "bit", nullable: false),
                    WsAtDocument = table.Column<bool>(type: "bit", nullable: false),
                    SaftAuditFile = table.Column<bool>(type: "bit", nullable: false),
                    SaftDocumentType = table.Column<int>(type: "int", nullable: false),
                    StockMode = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FiscalYears",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Acronym = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeriesForEachTerminal = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiscalYears", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Holidays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Year = table.Column<long>(type: "bigint", nullable: false),
                    Month = table.Column<long>(type: "bigint", nullable: false),
                    Day = table.Column<long>(type: "bigint", nullable: false),
                    Fixed = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holidays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InputReaders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReaderSizes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InputReaders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Acronym = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovementTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VatDirectSelling = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovementTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentConditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Acronym = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentConditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResourceString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ButtonIcon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Acronym = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AllowPayback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PoleDisplays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndPoint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodeTable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    COMPort = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CharactersPerLine = table.Column<long>(type: "bigint", nullable: false),
                    GoToStandByInSeconds = table.Column<long>(type: "bigint", nullable: false),
                    StandByLine1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StandByLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoleDisplays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PreferenceParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValueTip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Required = table.Column<bool>(type: "bit", nullable: false),
                    RegEx = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResourceString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResourceStringInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormType = table.Column<int>(type: "int", nullable: false),
                    FormPageNo = table.Column<int>(type: "int", nullable: true),
                    InputType = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferenceParameters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnumValue = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrinterTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThermalPrinter = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrinterTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SizeUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SizeUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemAuditTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResourceString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAuditTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemBackups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DatabaseType = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileNamePacked = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemBackups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemNotificationTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarnDaysBefore = table.Column<int>(type: "int", nullable: false),
                    TargetUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetTerminalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemNotificationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VatExemptionReasons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Acronym = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StandardApplicable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VatExemptionReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VatRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(5,3)", nullable: false),
                    ReasonCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryRegion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VatRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeighingMachines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaudRate = table.Column<long>(type: "bigint", nullable: false),
                    Parity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StopBits = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataBits = table.Column<long>(type: "bigint", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeighingMachines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkSessionPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkSessionPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticleFamilies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommissionGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DiscountGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Button_Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Button_ImageExtension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Button_Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleFamilies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleFamilies_CommissionGroups_CommissionGroupId",
                        column: x => x.CommissionGroupId,
                        principalTable: "CommissionGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArticleFamilies_DiscountGroups_DiscountGroupId",
                        column: x => x.DiscountGroupId,
                        principalTable: "DiscountGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentSeries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NextNumber = table.Column<int>(type: "int", nullable: false),
                    NumberRangeBegin = table.Column<int>(type: "int", nullable: false),
                    NumberRangeEnd = table.Column<int>(type: "int", nullable: false),
                    Acronym = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FiscalYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ATDocCodeValidationSeries = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentSeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentSeries_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentSeries_FiscalYears_FiscalYearId",
                        column: x => x.FiscalYearId,
                        principalTable: "FiscalYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hash4Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Atcud = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusSourceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourcePayment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mechanism = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountInWords = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxPayable = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    WithholdingTaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QrCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receipts_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PermissionItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermissionGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionItems_PermissionGroups_PermissionGroupId",
                        column: x => x.PermissionGroupId,
                        principalTable: "PermissionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscountGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PriceTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Locality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobilePhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebSite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FiscalNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscountType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discount = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CardCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Supplier = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Customers_CustomerTypes_CustomerTypeId",
                        column: x => x.CustomerTypeId,
                        principalTable: "CustomerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Customers_DiscountGroups_DiscountGroupId",
                        column: x => x.DiscountGroupId,
                        principalTable: "DiscountGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Customers_PriceTypes_PriceTypeId",
                        column: x => x.PriceTypeId,
                        principalTable: "PriceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriceTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovementTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ButtonImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeSubtotal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderPrintMode = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Places_MovementTypes_MovementTypeId",
                        column: x => x.MovementTypeId,
                        principalTable: "MovementTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Places_PriceTypes_PriceTypeId",
                        column: x => x.PriceTypeId,
                        principalTable: "PriceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Printers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NetworkName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThermalEncoding = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThermalPrintLogo = table.Column<bool>(type: "bit", nullable: true),
                    ThermalImageCompanyLogo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThermalMaxCharsPerLineNormal = table.Column<int>(type: "int", nullable: true),
                    ThermalMaxCharsPerLineNormalBold = table.Column<int>(type: "int", nullable: true),
                    ThermalMaxCharsPerLineSmall = table.Column<int>(type: "int", nullable: true),
                    ThermalCutCommand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThermalOpenDrawerValueM = table.Column<int>(type: "int", nullable: true),
                    ThermalOpenDrawerValueT1 = table.Column<int>(type: "int", nullable: true),
                    ThermalOpenDrawerValueT2 = table.Column<int>(type: "int", nullable: true),
                    ShowInDialog = table.Column<bool>(type: "bit", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Printers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Printers_PrinterTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "PrinterTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuditTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemAudits_SystemAuditTypes_AuditTypeId",
                        column: x => x.AuditTypeId,
                        principalTable: "SystemAuditTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommissionGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Residence = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Locality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZipCpde = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfContract = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobilePhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FiscalNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedSeating = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessPin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccessCardNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Login = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordReset = table.Column<bool>(type: "bit", nullable: false),
                    PasswordResetDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BaseConsumption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaseOffers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PVPOffers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ButtonImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_CommissionGroups_CommissionGroupId",
                        column: x => x.CommissionGroupId,
                        principalTable: "CommissionGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_UserProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseLocations_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticleSubfamilies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommissionGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DiscountGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VatOnTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VatDirectSellingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Button_Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Button_ImageExtension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Button_Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleSubfamilies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleSubfamilies_ArticleFamilies_FamilyId",
                        column: x => x.FamilyId,
                        principalTable: "ArticleFamilies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleSubfamilies_CommissionGroups_CommissionGroupId",
                        column: x => x.CommissionGroupId,
                        principalTable: "CommissionGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArticleSubfamilies_DiscountGroups_DiscountGroupId",
                        column: x => x.DiscountGroupId,
                        principalTable: "DiscountGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArticleSubfamilies_VatRates_VatDirectSellingId",
                        column: x => x.VatDirectSellingId,
                        principalTable: "VatRates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArticleSubfamilies_VatRates_VatOnTableId",
                        column: x => x.VatOnTableId,
                        principalTable: "VatRates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PermissionProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Granted = table.Column<bool>(type: "bit", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionProfiles_PermissionItems_PermissionItemId",
                        column: x => x.PermissionItemId,
                        principalTable: "PermissionItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionProfiles_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ButtonImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discount = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OpennedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tables_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrinterAssociations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrinterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrinterAssociations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrinterAssociations_Printers_PrinterId",
                        column: x => x.PrinterId,
                        principalTable: "Printers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Terminals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrinterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WeighingMachineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ThermalPrinterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BarcodeReaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CardReaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PoleDisplayId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HardwareId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimerInterval = table.Column<long>(type: "bigint", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terminals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Terminals_InputReaders_BarcodeReaderId",
                        column: x => x.BarcodeReaderId,
                        principalTable: "InputReaders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Terminals_InputReaders_CardReaderId",
                        column: x => x.CardReaderId,
                        principalTable: "InputReaders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Terminals_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Terminals_PoleDisplays_PoleDisplayId",
                        column: x => x.PoleDisplayId,
                        principalTable: "PoleDisplays",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Terminals_Printers_PrinterId",
                        column: x => x.PrinterId,
                        principalTable: "Printers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Terminals_Printers_ThermalPrinterId",
                        column: x => x.ThermalPrinterId,
                        principalTable: "Printers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Terminals_WeighingMachines_WeighingMachineId",
                        column: x => x.WeighingMachineId,
                        principalTable: "WeighingMachines",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubfamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MeasurementUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SizeUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommissionGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DiscountGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VatOnTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VatDirectSellingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VatExemptionReasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CodeDealer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceWithVat = table.Column<bool>(type: "bit", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    DefaultQuantity = table.Column<decimal>(type: "decimal(10,5)", nullable: false),
                    MinimumStock = table.Column<decimal>(type: "decimal(10,5)", nullable: false),
                    Tare = table.Column<float>(type: "real", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PVPVariable = table.Column<bool>(type: "bit", nullable: false),
                    Favorite = table.Column<bool>(type: "bit", nullable: false),
                    UseWeighingBalance = table.Column<bool>(type: "bit", nullable: false),
                    IsComposed = table.Column<bool>(type: "bit", nullable: false),
                    UniqueArticles = table.Column<bool>(type: "bit", nullable: false),
                    Button_Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Button_ImageExtension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Button_Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price1_PromotionValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price1_UsePromotion = table.Column<bool>(type: "bit", nullable: false),
                    Price1_Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price2_PromotionValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price2_UsePromotion = table.Column<bool>(type: "bit", nullable: false),
                    Price2_Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price3_PromotionValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price3_UsePromotion = table.Column<bool>(type: "bit", nullable: false),
                    Price3_Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price4_PromotionValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price4_UsePromotion = table.Column<bool>(type: "bit", nullable: false),
                    Price4_Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price5_PromotionValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price5_UsePromotion = table.Column<bool>(type: "bit", nullable: false),
                    Price5_Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_ArticleClasses_ClassId",
                        column: x => x.ClassId,
                        principalTable: "ArticleClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Articles_ArticleSubfamilies_SubfamilyId",
                        column: x => x.SubfamilyId,
                        principalTable: "ArticleSubfamilies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Articles_ArticleTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "ArticleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Articles_CommissionGroups_CommissionGroupId",
                        column: x => x.CommissionGroupId,
                        principalTable: "CommissionGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Articles_DiscountGroups_DiscountGroupId",
                        column: x => x.DiscountGroupId,
                        principalTable: "DiscountGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Articles_MeasurementUnits_MeasurementUnitId",
                        column: x => x.MeasurementUnitId,
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Articles_SizeUnits_SizeUnitId",
                        column: x => x.SizeUnitId,
                        principalTable: "SizeUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Articles_VatExemptionReasons_VatExemptionReasonId",
                        column: x => x.VatExemptionReasonId,
                        principalTable: "VatExemptionReasons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Articles_VatRates_VatDirectSellingId",
                        column: x => x.VatDirectSellingId,
                        principalTable: "VatRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Articles_VatRates_VatOnTableId",
                        column: x => x.VatOnTableId,
                        principalTable: "VatRates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderStatus = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ord = table.Column<long>(type: "bigint", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetTerminalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastReadUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastReadTerminalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemNotifications_SystemNotificationTypes_NotificationTypeId",
                        column: x => x.NotificationTypeId,
                        principalTable: "SystemNotificationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SystemNotifications_Terminals_LastReadTerminalId",
                        column: x => x.LastReadTerminalId,
                        principalTable: "Terminals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SystemNotifications_Terminals_TargetTerminalId",
                        column: x => x.TargetTerminalId,
                        principalTable: "Terminals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SystemNotifications_Users_LastReadUserId",
                        column: x => x.LastReadUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SystemNotifications_Users_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ArticleCompositions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChildId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,5)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleCompositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleCompositions_Articles_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Articles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArticleCompositions_Articles_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Articles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WarehouseArticles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    WarehouseLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,5)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseArticles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseArticles_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WarehouseArticles_WarehouseLocations_WarehouseLocationId",
                        column: x => x.WarehouseLocationId,
                        principalTable: "WarehouseLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentConditionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceBilling = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HashControl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SelfBillingIndicator = table.Column<int>(type: "int", nullable: false),
                    CashVatSchemeIndicator = table.Column<int>(type: "int", nullable: false),
                    ThirdPartiesBillingIndicator = table.Column<int>(type: "int", nullable: false),
                    EACCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    MovementStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MovementEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalNet = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalFinal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDelivery = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalChange = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(5,3)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    WithholdingTaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Paid = table.Column<bool>(type: "bit", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ATDocCodeID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ATValidAuditResult = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ATResendDocument = table.Column<bool>(type: "bit", nullable: true),
                    ATCUD = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ATQRCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer_Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer_Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer_CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Customer_Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer_Fax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer_FiscalNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Customer_Locality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Customer_Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer_ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipFromAddress_AddressDetail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipFromAddress_BuildingNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipFromAddress_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipFromAddress_Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipFromAddress_DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShipFromAddress_DeliveryID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipFromAddress_LocationID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipFromAddress_PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipFromAddress_Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipFromAddress_StreetName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipFromAddress_WarehouseID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipToAddress_AddressDetail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipToAddress_BuildingNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipToAddress_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipToAddress_Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipToAddress_DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShipToAddress_DeliveryID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipToAddress_LocationID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipToAddress_PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipToAddress_Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipToAddress_StreetName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipToAddress_WarehouseID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Documents_DocumentSeries_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "DocumentSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Documents_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Documents_PaymentConditions_PaymentConditionId",
                        column: x => x.PaymentConditionId,
                        principalTable: "PaymentConditions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    PriceType = table.Column<int>(type: "int", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(5,3)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UniqueArticleCompositions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChildId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniqueArticleCompositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UniqueArticleCompositions_WarehouseArticles_ChildId",
                        column: x => x.ChildId,
                        principalTable: "WarehouseArticles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UniqueArticleCompositions_WarehouseArticles_ParentId",
                        column: x => x.ParentId,
                        principalTable: "WarehouseArticles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AtAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    PostData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnCode = table.Column<int>(type: "int", nullable: false),
                    ReturnMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnRaw = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ATDocCodeID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AtAudits_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,5)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatExemptionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VatExemptionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discount = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TotalNet = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalFinal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceType = table.Column<int>(type: "int", nullable: true),
                    Tax_Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tax_CountryRegion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tax_Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tax_Designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tax_Percentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Tax_TaxId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tax_Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentDetails_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentDetails_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentPaymentMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentPaymentMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentPaymentMethods_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentPaymentMethods_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_Receipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "Receipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VatExemptionReasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,5)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Vat = table.Column<decimal>(type: "decimal(5,3)", nullable: false),
                    TotalDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalFinal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_VatExemptionReasons_VatExemptionReasonId",
                        column: x => x.VatExemptionReasonId,
                        principalTable: "VatExemptionReasons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Commissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommissionValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Commissions_CommissionGroups_CommissionGroupId",
                        column: x => x.CommissionGroupId,
                        principalTable: "CommissionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Commissions_DocumentDetails_DocumentDetailId",
                        column: x => x.DocumentDetailId,
                        principalTable: "DocumentDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Commissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarehouseLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DocumentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(10,5)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalDocument = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockMovements_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockMovements_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockMovements_DocumentDetails_DetailId",
                        column: x => x.DetailId,
                        principalTable: "DocumentDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockMovements_WarehouseLocations_WarehouseLocationId",
                        column: x => x.WarehouseLocationId,
                        principalTable: "WarehouseLocations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkSessionMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedWhere = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkSessionMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkSessionMovements_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkSessionMovements_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkSessionMovements_WorkSessionPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "WorkSessionPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCompositions_ChildId",
                table: "ArticleCompositions",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCompositions_ParentId",
                table: "ArticleCompositions",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleFamilies_CommissionGroupId",
                table: "ArticleFamilies",
                column: "CommissionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleFamilies_DiscountGroupId",
                table: "ArticleFamilies",
                column: "DiscountGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ClassId",
                table: "Articles",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CommissionGroupId",
                table: "Articles",
                column: "CommissionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_DiscountGroupId",
                table: "Articles",
                column: "DiscountGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_MeasurementUnitId",
                table: "Articles",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_SizeUnitId",
                table: "Articles",
                column: "SizeUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_SubfamilyId",
                table: "Articles",
                column: "SubfamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_TypeId",
                table: "Articles",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_VatDirectSellingId",
                table: "Articles",
                column: "VatDirectSellingId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_VatExemptionReasonId",
                table: "Articles",
                column: "VatExemptionReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_VatOnTableId",
                table: "Articles",
                column: "VatOnTableId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleSubfamilies_CommissionGroupId",
                table: "ArticleSubfamilies",
                column: "CommissionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleSubfamilies_DiscountGroupId",
                table: "ArticleSubfamilies",
                column: "DiscountGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleSubfamilies_FamilyId",
                table: "ArticleSubfamilies",
                column: "FamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleSubfamilies_VatDirectSellingId",
                table: "ArticleSubfamilies",
                column: "VatDirectSellingId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleSubfamilies_VatOnTableId",
                table: "ArticleSubfamilies",
                column: "VatOnTableId");

            migrationBuilder.CreateIndex(
                name: "IX_AtAudits_DocumentId",
                table: "AtAudits",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_CommissionGroupId",
                table: "Commissions",
                column: "CommissionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_DocumentDetailId",
                table: "Commissions",
                column: "DocumentDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_UserId",
                table: "Commissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CountryId",
                table: "Customers",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerTypeId",
                table: "Customers",
                column: "CustomerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_DiscountGroupId",
                table: "Customers",
                column: "DiscountGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PriceTypeId",
                table: "Customers",
                column: "PriceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDetails_ArticleId",
                table: "DocumentDetails",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDetails_DocumentId",
                table: "DocumentDetails",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPaymentMethods_DocumentId",
                table: "DocumentPaymentMethods",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPaymentMethods_PaymentMethodId",
                table: "DocumentPaymentMethods",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CurrencyId",
                table: "Documents",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_OrderId",
                table: "Documents",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PaymentConditionId",
                table: "Documents",
                column: "PaymentConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_SeriesId",
                table: "Documents",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSeries_DocumentTypeId",
                table: "DocumentSeries",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSeries_FiscalYearId",
                table: "DocumentSeries",
                column: "FiscalYearId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ArticleId",
                table: "OrderDetails",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_TicketId",
                table: "OrderDetails",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_VatExemptionReasonId",
                table: "OrderDetails",
                column: "VatExemptionReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TableId",
                table: "Orders",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_DocumentId",
                table: "Payments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ReceiptId",
                table: "Payments",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionItems_PermissionGroupId",
                table: "PermissionItems",
                column: "PermissionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionProfiles_PermissionItemId",
                table: "PermissionProfiles",
                column: "PermissionItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionProfiles_UserProfileId",
                table: "PermissionProfiles",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Places_MovementTypeId",
                table: "Places",
                column: "MovementTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Places_PriceTypeId",
                table: "Places",
                column: "PriceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PrinterAssociations_PrinterId",
                table: "PrinterAssociations",
                column: "PrinterId");

            migrationBuilder.CreateIndex(
                name: "IX_Printers_TypeId",
                table: "Printers",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_PaymentMethodId",
                table: "Receipts",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ArticleId",
                table: "StockMovements",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_CustomerId",
                table: "StockMovements",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_DetailId",
                table: "StockMovements",
                column: "DetailId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_WarehouseLocationId",
                table: "StockMovements",
                column: "WarehouseLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemAudits_AuditTypeId",
                table: "SystemAudits",
                column: "AuditTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemNotifications_LastReadTerminalId",
                table: "SystemNotifications",
                column: "LastReadTerminalId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemNotifications_LastReadUserId",
                table: "SystemNotifications",
                column: "LastReadUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemNotifications_NotificationTypeId",
                table: "SystemNotifications",
                column: "NotificationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemNotifications_TargetTerminalId",
                table: "SystemNotifications",
                column: "TargetTerminalId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemNotifications_TargetUserId",
                table: "SystemNotifications",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_PlaceId",
                table: "Tables",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminals_BarcodeReaderId",
                table: "Terminals",
                column: "BarcodeReaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminals_CardReaderId",
                table: "Terminals",
                column: "CardReaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminals_PlaceId",
                table: "Terminals",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminals_PoleDisplayId",
                table: "Terminals",
                column: "PoleDisplayId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminals_PrinterId",
                table: "Terminals",
                column: "PrinterId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminals_ThermalPrinterId",
                table: "Terminals",
                column: "ThermalPrinterId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminals_WeighingMachineId",
                table: "Terminals",
                column: "WeighingMachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_OrderId",
                table: "Tickets",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueArticleCompositions_ChildId",
                table: "UniqueArticleCompositions",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueArticleCompositions_ParentId",
                table: "UniqueArticleCompositions",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CommissionGroupId",
                table: "Users",
                column: "CommissionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProfileId",
                table: "Users",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseArticles_ArticleId",
                table: "WarehouseArticles",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseArticles_WarehouseLocationId",
                table: "WarehouseArticles",
                column: "WarehouseLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseLocations_WarehouseId",
                table: "WarehouseLocations",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSessionMovements_DocumentId",
                table: "WorkSessionMovements",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSessionMovements_PaymentId",
                table: "WorkSessionMovements",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSessionMovements_PeriodId",
                table: "WorkSessionMovements",
                column: "PeriodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleCompositions");

            migrationBuilder.DropTable(
                name: "AtAudits");

            migrationBuilder.DropTable(
                name: "Commissions");

            migrationBuilder.DropTable(
                name: "DocumentNotifications");

            migrationBuilder.DropTable(
                name: "DocumentPaymentMethods");

            migrationBuilder.DropTable(
                name: "DocumentPrintings");

            migrationBuilder.DropTable(
                name: "Holidays");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "PermissionProfiles");

            migrationBuilder.DropTable(
                name: "PreferenceParameters");

            migrationBuilder.DropTable(
                name: "PrinterAssociations");

            migrationBuilder.DropTable(
                name: "StockMovements");

            migrationBuilder.DropTable(
                name: "SystemAudits");

            migrationBuilder.DropTable(
                name: "SystemBackups");

            migrationBuilder.DropTable(
                name: "SystemNotifications");

            migrationBuilder.DropTable(
                name: "UniqueArticleCompositions");

            migrationBuilder.DropTable(
                name: "WorkSessionMovements");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "PermissionItems");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "DocumentDetails");

            migrationBuilder.DropTable(
                name: "SystemAuditTypes");

            migrationBuilder.DropTable(
                name: "SystemNotificationTypes");

            migrationBuilder.DropTable(
                name: "Terminals");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WarehouseArticles");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "WorkSessionPeriods");

            migrationBuilder.DropTable(
                name: "PermissionGroups");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "CustomerTypes");

            migrationBuilder.DropTable(
                name: "InputReaders");

            migrationBuilder.DropTable(
                name: "PoleDisplays");

            migrationBuilder.DropTable(
                name: "Printers");

            migrationBuilder.DropTable(
                name: "WeighingMachines");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "WarehouseLocations");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Receipts");

            migrationBuilder.DropTable(
                name: "PrinterTypes");

            migrationBuilder.DropTable(
                name: "ArticleClasses");

            migrationBuilder.DropTable(
                name: "ArticleSubfamilies");

            migrationBuilder.DropTable(
                name: "ArticleTypes");

            migrationBuilder.DropTable(
                name: "MeasurementUnits");

            migrationBuilder.DropTable(
                name: "SizeUnits");

            migrationBuilder.DropTable(
                name: "VatExemptionReasons");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "DocumentSeries");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "PaymentConditions");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "ArticleFamilies");

            migrationBuilder.DropTable(
                name: "VatRates");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropTable(
                name: "FiscalYears");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropTable(
                name: "CommissionGroups");

            migrationBuilder.DropTable(
                name: "DiscountGroups");

            migrationBuilder.DropTable(
                name: "Places");

            migrationBuilder.DropTable(
                name: "MovementTypes");

            migrationBuilder.DropTable(
                name: "PriceTypes");
        }
    }
}

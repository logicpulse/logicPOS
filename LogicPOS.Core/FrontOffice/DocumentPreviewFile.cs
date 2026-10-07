using LogicPOS.Application.Features.Finance.Documents.PdfGeneration;
using LogicPOS.Core.Fiscal;
using LogicPOS.Domain.ValueObjects;
using LogicPOS.Shared.Features.Company;
using PdfDetail = LogicPOS.Application.Features.Finance.Documents.PdfGeneration.DocumentDetail;
using PdfDocument = LogicPOS.Application.Features.Finance.Documents.PdfGeneration.Document;
using PdfPayment = LogicPOS.Application.Features.Finance.Documents.PdfGeneration.PaymentMethod;

namespace LogicPOS.Core.FrontOffice;

public sealed class DocumentPreviewLine
{
    public string Code { get; init; } = string.Empty;

    public string Designation { get; init; } = string.Empty;

    public decimal Quantity { get; init; }

    public decimal NetUnitPrice { get; init; }

    public decimal Discount { get; init; }

    public decimal VatPercentage { get; init; }

    public decimal TotalNet { get; init; }

    public decimal TotalFinal { get; init; }

    public string? Notes { get; init; }
}

public sealed class DocumentPreview
{
    public string Acronym { get; init; } = string.Empty;

    public string TypeName { get; init; } = string.Empty;

    public string CustomerName { get; init; } = string.Empty;

    public string FiscalNumber { get; init; } = string.Empty;

    public string? Address { get; init; }

    public string? City { get; init; }

    public string? PostalCode { get; init; }

    public string? Country { get; init; }

    public string? Notes { get; init; }

    public string? PaymentCondition { get; init; }

    public string Currency { get; init; } = "EUR";

    public string? CompanyAddress { get; init; }

    public string? CompanyCity { get; init; }

    public string? CompanyPostalCode { get; init; }

    public string? CompanyCountry { get; init; }

    public ShipAddress? ShipTo { get; init; }

    public ShipAddress? ShipFrom { get; init; }

    public IReadOnlyList<DocumentPreviewLine> Lines { get; init; } = [];
}

public static class DocumentPreviewFile
{
    public static string Write(DocumentPreview preview)
    {
        var details = preview.Lines.Select((line, index) => new PdfDetail
        {
            CreatedAt = DateTime.Now,
            Order = (uint)(index + 1),
            Code = line.Code,
            Designation = line.Designation,
            Quantity = line.Quantity,
            Price = line.NetUnitPrice,
            Discount = line.Discount,
            TotalNet = line.TotalNet,
            TotalTax = line.TotalFinal - line.TotalNet,
            TotalFinal = line.TotalFinal,
            Notes = line.Notes,
            ArticleClassAcronym = string.Empty,
            Tax = new DocumentDetailTax
            {
                Designation = "IVA",
                Type = "IVA",
                Code = "NOR",
                Description = "IVA",
                Percentage = line.VatPercentage,
                CountryRegion = "PT"
            }
        }).ToList();
        var totalNet = details.Sum(detail => detail.TotalNet);
        var totalFinal = details.Sum(detail => detail.TotalFinal);
        var document = new PdfDocument
        {
            Type = preview.Acronym,
            Status = "N",
            IsDraft = true,
            CreatedAt = DateTime.Now,
            Number = "Pré-visualização",
            TypeName = preview.TypeName,
            CustomerId = Guid.Empty,
            Customer = new DocumentCustomer
            {
                Name = string.IsNullOrWhiteSpace(preview.CustomerName) ? "Consumidor Final" : preview.CustomerName,
                FiscalNumber = preview.FiscalNumber,
                Address = preview.Address,
                City = preview.City,
                ZipCode = preview.PostalCode,
                Country = preview.Country
            },
            Currency = string.IsNullOrWhiteSpace(preview.Currency) ? "EUR" : preview.Currency,
            Hash = string.Empty,
            Notes = preview.Notes,
            PaymentCondition = preview.PaymentCondition,
            TotalNet = totalNet,
            TotalTax = totalFinal - totalNet,
            TotalFinal = totalFinal,
            Details = details,
            RelatedDocuments = [],
            ShipToAddress = preview.ShipTo ?? new ShipAddress(),
            ShipFromAddress = preview.ShipFrom ?? new ShipAddress(),
            PaymentMethods =
            [
                new PdfPayment
                {
                    Designation = "Pré-visualização",
                    Acronym = "PREV",
                    Amount = totalFinal
                }
            ]
        };
        var data = new DocumentPdfData
        {
            Company = new CompanyInformation
            {
                Address = preview.CompanyAddress,
                City = preview.CompanyCity,
                PostalCode = preview.CompanyPostalCode,
                CountryCode2 = preview.CompanyCountry,
                CurrencyCode = document.Currency
            },
            Country = FiscalCountry.Portugal,
            Document = document,
            SoftwareVersion = "preview"
        };
        var path = Path.Combine(Path.GetTempPath(), "logicpos-preview.pdf");
        File.WriteAllBytes(path, A4DocumentPdf.Render(data));
        return path;
    }
}

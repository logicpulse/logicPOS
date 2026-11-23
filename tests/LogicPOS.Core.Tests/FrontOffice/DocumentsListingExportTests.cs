using LogicPOS.Core.FrontOffice;

namespace LogicPOS.Core.Tests.FrontOffice;

public sealed class DocumentsListingExportTests
{
    [Fact]
    public void Cells_resolve_by_stable_key_when_headers_are_english()
    {
        var row = new PosDocumentRow
        {
            Id = Guid.NewGuid(),
            DocumentDate = new DateTime(2026, 11, 23, 10, 15, 30),
            Number = "FS A001/1",
            Status = "Normal",
            EntityName = "ACME Lda",
            TaxId = "123456789",
            FinalTotal = 12.34m,
            PaidAmount = 12.34m,
            DebitAmount = 0m,
            AssociatedDocuments = "RC A001/2"
        };

        // Headers would be translated in EN; export must still use SortMemberPath keys.
        Assert.Equal(row.DocumentDateText, DocumentsListingExport.Cell(row, "DocumentDateText"));
        Assert.Equal("FS A001/1", DocumentsListingExport.Cell(row, "Number"));
        Assert.Equal("Normal", DocumentsListingExport.Cell(row, "Status"));
        Assert.Equal("ACME Lda", DocumentsListingExport.Cell(row, "EntityName"));
        Assert.Equal("123456789", DocumentsListingExport.Cell(row, "TaxId"));
        Assert.False(string.IsNullOrWhiteSpace(DocumentsListingExport.Cell(row, "FinalTotalText")));
        Assert.False(string.IsNullOrWhiteSpace(DocumentsListingExport.Cell(row, "PaidAmountText")));
        Assert.Equal(row.DebitAmountText, DocumentsListingExport.Cell(row, "DebitAmountText"));
        Assert.Equal("RC A001/2", DocumentsListingExport.Cell(row, "AssociatedDocuments"));

        // Translated captions must not be used as keys.
        Assert.Equal(string.Empty, DocumentsListingExport.Cell(row, "Document Date"));
        Assert.Equal(string.Empty, DocumentsListingExport.Cell(row, "Data do documento"));
    }

    [Fact]
    public void Export_row_for_en_US_headers_still_has_values_via_keys()
    {
        var row = new PosDocumentRow
        {
            DocumentDate = new DateTime(2026, 1, 2, 8, 0, 0),
            Number = "FT B/9",
            Status = "N",
            EntityName = "Buyer",
            TaxId = "999",
            FinalTotal = 1m,
            PaidAmount = 1m,
            DebitAmount = 0m,
            AssociatedDocuments = "-"
        };

        var keys = new[]
        {
            "DocumentDateText", "Number", "Status", "EntityName", "TaxId",
            "FinalTotalText", "PaidAmountText", "DebitAmountText", "AssociatedDocuments"
        };
        var enHeaders = new[]
        {
            "Document Date", "Document Number", "Status", "Entity", "SSN/EIN",
            "Total Final", "Credit Value", "Debit", "Associated Documents"
        };

        var cells = keys.Select(key => DocumentsListingExport.Cell(row, key)).ToList();
        Assert.Equal(enHeaders.Length, cells.Count);
        Assert.All(cells, cell => Assert.False(string.IsNullOrWhiteSpace(cell)));
        Assert.Equal("FT B/9", cells[1]);
        Assert.Equal("Buyer", cells[3]);
    }

    [Fact]
    public void ResolveRows_prefers_selection_otherwise_all_matching()
    {
        var visible = new PosDocumentRow { Number = "A" };
        var other = new PosDocumentRow { Number = "B" };
        var selected = DocumentsListingExport.ResolveRows([visible], [visible, other]);
        Assert.Equal(["A"], selected.Select(row => row.Number));

        var all = DocumentsListingExport.ResolveRows([], [visible, other]);
        Assert.Equal(["A", "B"], all.Select(row => row.Number));
    }

    [Fact]
    public void Excel_bytes_include_header_and_data_rows()
    {
        var row = new PosDocumentRow
        {
            DocumentDate = new DateTime(2026, 11, 23, 10, 0, 0),
            Number = "FS A/1",
            Status = "Normal",
            EntityName = "ACME",
            TaxId = "123",
            FinalTotal = 10m,
            PaidAmount = 10m,
            DebitAmount = 0m,
            AssociatedDocuments = "-"
        };
        var keys = new[]
        {
            "DocumentDateText", "Number", "Status", "EntityName", "TaxId",
            "FinalTotalText", "PaidAmountText", "DebitAmountText", "AssociatedDocuments"
        };
        var headers = new[]
        {
            "Data do documento", "Número do Doc.", "Estado", "Entidade", "NIF",
            "Total Final", "Valor Pago", "Valor em Dívida", "Documentos Associados"
        };
        var data = new[] { DocumentsListingExport.BuildRow(row, keys) };
        var bytes = LogicPOS.Core.BackOffice.ListingTableExport.ToExcel("Documentos", headers, data);
        var xml = ReadSheetXml(bytes);
        Assert.Contains("Data do documento", xml, StringComparison.Ordinal);
        Assert.Contains("FS A/1", xml, StringComparison.Ordinal);
        Assert.Contains("ACME", xml, StringComparison.Ordinal);
        Assert.Contains("r=\"2\"", xml, StringComparison.Ordinal);
    }

    private static string ReadSheetXml(byte[] xlsx)
    {
        using var stream = new MemoryStream(xlsx);
        using var zip = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Read);
        using var reader = new StreamReader(zip.GetEntry("xl/worksheets/sheet1.xml")!.Open());
        return reader.ReadToEnd();
    }
}

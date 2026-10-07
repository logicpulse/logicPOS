using System.IO.Compression;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LogicPOS.Core.BackOffice;

public static class ListingTableExport
{
    public static byte[] ToExcel(string title, IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string>> rows)
    {
        using var stream = new MemoryStream();
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, true))
        {
            Write(zip, "[Content_Types].xml", ContentTypes());
            Write(zip, "_rels/.rels", PackageRels());
            Write(zip, "xl/workbook.xml", Workbook(title));
            Write(zip, "xl/_rels/workbook.xml.rels", WorkbookRels());
            Write(zip, "xl/styles.xml", Styles());
            Write(zip, "xl/worksheets/sheet1.xml", Sheet(headers, rows));
        }

        return stream.ToArray();
    }

    public static byte[] ToPdf(string title, IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string>> rows)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var fontSize = headers.Count > 8 ? 8 : 10;
        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(24);
                page.DefaultTextStyle(style => style.FontSize(fontSize).FontFamily("Arial"));
                page.Header().PaddingBottom(8).Text(title).FontSize(14).SemiBold();
                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        foreach (var _ in headers)
                        {
                            columns.RelativeColumn();
                        }
                    });
                    table.Header(header =>
                    {
                        foreach (var name in headers)
                        {
                            header.Cell().Background("#F7F7F7").Padding(4).Text(name).SemiBold();
                        }
                    });
                    foreach (var row in rows)
                    {
                        for (var index = 0; index < headers.Count; index++)
                        {
                            var value = index < row.Count ? row[index] : string.Empty;
                            table.Cell().BorderBottom(0.4f).BorderColor("#EEEEEE").Padding(3).Text(value);
                        }
                    }
                });
            });
        }).GeneratePdf();
    }

    private static void Write(ZipArchive zip, string name, string content)
    {
        var entry = zip.CreateEntry(name, CompressionLevel.Fastest);
        using var writer = new StreamWriter(entry.Open(), new UTF8Encoding(false));
        writer.Write(content);
    }

    private static string ContentTypes() =>
        """
        <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
        <Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
          <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
          <Default Extension="xml" ContentType="application/xml"/>
          <Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/>
          <Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>
          <Override PartName="/xl/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"/>
        </Types>
        """;

    private static string PackageRels() =>
        """
        <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
        <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
          <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/>
        </Relationships>
        """;

    private static string WorkbookRels() =>
        """
        <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
        <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
          <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/>
          <Relationship Id="rId2" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/>
        </Relationships>
        """;

    private static string Styles() =>
        """
        <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
        <styleSheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">
          <fonts count="1"><font><sz val="11"/><name val="Calibri"/></font></fonts>
          <fills count="1"><fill><patternFill patternType="none"/></fill></fills>
          <borders count="1"><border/></borders>
          <cellStyleXfs count="1"><xf numFmtId="0" fontId="0" fillId="0" borderId="0"/></cellStyleXfs>
          <cellXfs count="1"><xf numFmtId="0" fontId="0" fillId="0" borderId="0" xfId="0"/></cellXfs>
        </styleSheet>
        """;

    private static string Workbook(string title) =>
        $"""
        <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
        <workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
          <sheets><sheet name="{Xml(SheetName(title))}" sheetId="1" r:id="rId1"/></sheets>
        </workbook>
        """;

    private static string Sheet(IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string>> rows)
    {
        var builder = new StringBuilder();
        builder.Append("""<?xml version="1.0" encoding="UTF-8" standalone="yes"?>""");
        builder.Append("""<worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"><sheetData>""");
        AppendRow(builder, 1, headers);
        for (var index = 0; index < rows.Count; index++)
        {
            var values = new string[headers.Count];
            for (var column = 0; column < headers.Count; column++)
            {
                values[column] = column < rows[index].Count ? rows[index][column] : string.Empty;
            }

            AppendRow(builder, index + 2, values);
        }

        builder.Append("</sheetData></worksheet>");
        return builder.ToString();
    }

    private static void AppendRow(StringBuilder builder, int rowNumber, IReadOnlyList<string> values)
    {
        builder.Append("<row r=\"").Append(rowNumber).Append("\">");
        for (var index = 0; index < values.Count; index++)
        {
            builder.Append("<c r=\"").Append(ColumnName(index)).Append(rowNumber).Append("\" t=\"inlineStr\"><is><t>");
            builder.Append(Xml(values[index]));
            builder.Append("</t></is></c>");
        }

        builder.Append("</row>");
    }

    private static string ColumnName(int index)
    {
        var name = string.Empty;
        index++;
        while (index > 0)
        {
            index--;
            name = (char)('A' + (index % 26)) + name;
            index /= 26;
        }

        return name;
    }

    private static string SheetName(string title)
    {
        var name = string.IsNullOrWhiteSpace(title) ? "Listagem" : title;
        foreach (var invalid in new[] { '\\', '/', '*', '?', ':', '[', ']' })
        {
            name = name.Replace(invalid, ' ');
        }

        return name.Length <= 31 ? name : name[..31];
    }

    private static string Xml(string? value) => System.Security.SecurityElement.Escape(value ?? string.Empty) ?? string.Empty;
}

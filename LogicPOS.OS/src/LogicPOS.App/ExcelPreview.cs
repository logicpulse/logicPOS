using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Xml.Linq;

namespace LogicPOS.App;

// Reads and writes the first worksheet of an xlsx without a spreadsheet package.
// Column positions are preserved so the import API keeps reading the same cells.
internal static class ExcelPreview
{
    private const int MaxColumns = 64;

    internal sealed class Sheet
    {
        public required bool HasHeader { get; init; }

        public required IReadOnlyList<string> Headers { get; init; }

        public required List<string[]> Rows { get; init; }
    }

    public static Sheet Read(byte[] content)
    {
        using var zip = new ZipArchive(new MemoryStream(content), ZipArchiveMode.Read);
        var shared = ReadSharedStrings(zip);
        var entry = zip.GetEntry(FirstSheetPath(zip)) ?? zip.GetEntry("xl/worksheets/sheet1.xml");
        if (entry is null)
        {
            throw new InvalidDataException("Worksheet not found.");
        }

        XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        var document = XDocument.Load(entry.Open());
        var rows = new List<string[]>();
        foreach (var row in document.Descendants(ns + "row"))
        {
            var values = new Dictionary<int, string>();
            var next = 0;
            foreach (var cell in row.Elements(ns + "c"))
            {
                var column = ColumnIndex(cell.Attribute("r")?.Value, next);
                if (column < 0 || column >= MaxColumns)
                {
                    continue;
                }

                next = column + 1;
                values[column] = CellText(cell, ns, shared);
            }

            if (values.Count == 0 || values.Values.All(string.IsNullOrWhiteSpace))
            {
                continue;
            }

            var width = values.Keys.Max() + 1;
            var cells = new string[width];
            for (var index = 0; index < width; index++)
            {
                cells[index] = values.TryGetValue(index, out var value) ? value : string.Empty;
            }

            rows.Add(cells);
        }

        if (rows.Count == 0)
        {
            return new Sheet { HasHeader = false, Headers = [], Rows = [] };
        }

        var columnCount = rows.Max(row => row.Length);
        var hasHeader = string.Equals(rows[0][0], "Code", StringComparison.OrdinalIgnoreCase);
        var headerSource = hasHeader ? rows[0] : [];
        var headers = new string[columnCount];
        for (var index = 0; index < columnCount; index++)
        {
            var name = index < headerSource.Length ? headerSource[index] : string.Empty;
            headers[index] = string.IsNullOrWhiteSpace(name) ? $"Coluna {index + 1}" : name;
        }

        var data = hasHeader ? rows.Skip(1) : rows;
        return new Sheet
        {
            HasHeader = hasHeader,
            Headers = headers,
            Rows = data.Select(row => Fit(row, columnCount)).ToList()
        };
    }

    public static byte[] Write(IReadOnlyList<string> headers, IEnumerable<string[]> rows, bool includeHeader)
    {
        var data = rows.Select(row => (string[])row.Clone()).ToList();
        var width = Math.Max(headers.Count, data.Count == 0 ? 0 : data.Max(row => row.Length));
        if (width == 0)
        {
            width = 1;
        }

        var sheet = new StringBuilder();
        sheet.Append("""<?xml version="1.0" encoding="UTF-8" standalone="yes"?><worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"><sheetData>""");
        var rowNumber = 1;
        if (includeHeader)
        {
            AppendRow(sheet, rowNumber++, Fit(headers.ToArray(), width));
        }

        foreach (var row in data)
        {
            AppendRow(sheet, rowNumber++, Fit(row, width));
        }

        sheet.Append("</sheetData></worksheet>");

        using var memory = new MemoryStream();
        using (var zip = new ZipArchive(memory, ZipArchiveMode.Create, true))
        {
            WriteEntry(zip, "[Content_Types].xml", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
                  <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
                  <Default Extension="xml" ContentType="application/xml"/>
                  <Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/>
                  <Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>
                </Types>
                """);
            WriteEntry(zip, "_rels/.rels", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
                  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/>
                </Relationships>
                """);
            WriteEntry(zip, "xl/workbook.xml", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
                  <sheets><sheet name="Folha1" sheetId="1" r:id="rId1"/></sheets>
                </workbook>
                """);
            WriteEntry(zip, "xl/_rels/workbook.xml.rels", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
                  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/>
                </Relationships>
                """);
            WriteEntry(zip, "xl/worksheets/sheet1.xml", sheet.ToString());
        }

        return memory.ToArray();
    }

    private static void AppendRow(StringBuilder sheet, int rowNumber, string[] cells)
    {
        sheet.Append(CultureInfo.InvariantCulture, $"<row r=\"{rowNumber}\">");
        for (var index = 0; index < cells.Length; index++)
        {
            var value = cells[index] ?? string.Empty;
            var space = value.Length > 0 && (char.IsWhiteSpace(value[0]) || char.IsWhiteSpace(value[^1]))
                ? " xml:space=\"preserve\""
                : string.Empty;
            sheet.Append(CultureInfo.InvariantCulture, $"<c r=\"{ColumnName(index)}{rowNumber}\" t=\"inlineStr\"><is><t{space}>{Escape(value)}</t></is></c>");
        }

        sheet.Append("</row>");
    }

    private static List<string> ReadSharedStrings(ZipArchive zip)
    {
        var entry = zip.GetEntry("xl/sharedStrings.xml");
        if (entry is null)
        {
            return [];
        }

        XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        var document = XDocument.Load(entry.Open());
        return document.Descendants(ns + "si")
            .Select(item => string.Concat(item.Descendants(ns + "t").Select(node => node.Value)))
            .ToList();
    }

    private static string FirstSheetPath(ZipArchive zip)
    {
        var workbook = zip.GetEntry("xl/workbook.xml");
        var relationships = zip.GetEntry("xl/_rels/workbook.xml.rels");
        if (workbook is null || relationships is null)
        {
            return "xl/worksheets/sheet1.xml";
        }

        var workbookDocument = XDocument.Load(workbook.Open());
        var relationshipDocument = XDocument.Load(relationships.Open());
        XNamespace main = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        XNamespace office = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        XNamespace package = "http://schemas.openxmlformats.org/package/2006/relationships";
        var id = workbookDocument.Descendants(main + "sheet").FirstOrDefault()?.Attribute(office + "id")?.Value;
        var target = relationshipDocument.Descendants(package + "Relationship")
            .FirstOrDefault(node => node.Attribute("Id")?.Value == id)
            ?.Attribute("Target")?.Value;
        if (string.IsNullOrWhiteSpace(target))
        {
            return "xl/worksheets/sheet1.xml";
        }

        target = target.Replace('\\', '/');
        if (target.StartsWith('/'))
        {
            return target.TrimStart('/');
        }

        return target.StartsWith("xl/", StringComparison.OrdinalIgnoreCase) ? target : "xl/" + target.TrimStart('/');
    }

    private static string CellText(XElement cell, XNamespace ns, IReadOnlyList<string> shared)
    {
        var type = cell.Attribute("t")?.Value;
        if (type == "inlineStr")
        {
            return string.Concat(cell.Descendants(ns + "t").Select(node => node.Value)).Trim();
        }

        var raw = cell.Element(ns + "v")?.Value ?? string.Empty;
        if (type == "s" && int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var index) && index >= 0 && index < shared.Count)
        {
            return shared[index].Trim();
        }

        return raw.Trim();
    }

    private static int ColumnIndex(string? reference, int fallback)
    {
        if (string.IsNullOrWhiteSpace(reference))
        {
            return fallback;
        }

        var index = 0;
        var found = false;
        foreach (var character in reference)
        {
            if (character is < 'A' or > 'Z' and < 'a' or > 'z')
            {
                break;
            }

            found = true;
            index = index * 26 + (char.ToUpperInvariant(character) - 'A' + 1);
        }

        return found ? index - 1 : fallback;
    }

    private static string ColumnName(int index)
    {
        var name = string.Empty;
        var number = index + 1;
        while (number > 0)
        {
            number--;
            name = (char)('A' + number % 26) + name;
            number /= 26;
        }

        return name;
    }

    private static string[] Fit(string[] cells, int width)
    {
        if (cells.Length == width)
        {
            return cells;
        }

        var copy = new string[width];
        for (var index = 0; index < width; index++)
        {
            copy[index] = index < cells.Length ? cells[index] ?? string.Empty : string.Empty;
        }

        return copy;
    }

    private static string Escape(string value) =>
        value.Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal);

    private static void WriteEntry(ZipArchive zip, string name, string content)
    {
        var entry = zip.CreateEntry(name, CompressionLevel.Optimal);
        using var writer = new StreamWriter(entry.Open(), new UTF8Encoding(false));
        writer.Write(content);
    }
}

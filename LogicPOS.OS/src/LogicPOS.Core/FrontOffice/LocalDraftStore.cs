using System.Text.Json;

namespace LogicPOS.Core.FrontOffice;

public sealed class StoredDraft
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Acronym { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string FiscalNumber { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public Guid? PaymentConditionId { get; set; }

    public Guid? CurrencyId { get; set; }

    public Guid? ParentDocumentId { get; set; }

    public decimal Total { get; set; }

    public PosShipAddress? ShipTo { get; set; }

    public PosShipAddress? ShipFrom { get; set; }

    public List<StoredDraftLine> Lines { get; set; } = [];
}

public sealed class StoredDraftLine
{
    public Guid ArticleId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal Discount { get; set; }

    public Guid VatRateId { get; set; }

    public decimal VatPercentage { get; set; }

    public string? Notes { get; set; }
}

public static class LocalDraftStore
{
    private static readonly object Gate = new();
    private static readonly JsonSerializerOptions Json = new() { WriteIndented = true };

    public static StoredDraft? Find(Guid id)
    {
        lock (Gate)
        {
            return Read().FirstOrDefault(draft => draft.Id == id);
        }
    }

    public static Guid Save(StoredDraft draft)
    {
        lock (Gate)
        {
            var drafts = Read();
            if (draft.Id == Guid.Empty)
            {
                draft.Id = Guid.NewGuid();
            }

            if (draft.CreatedAt == default)
            {
                draft.CreatedAt = DateTime.Now;
            }

            var index = drafts.FindIndex(item => item.Id == draft.Id);
            if (index >= 0)
            {
                draft.CreatedAt = drafts[index].CreatedAt;
                drafts[index] = draft;
            }
            else
            {
                drafts.Add(draft);
            }

            Write(drafts);
            return draft.Id;
        }
    }

    public static bool Delete(Guid id)
    {
        lock (Gate)
        {
            var drafts = Read();
            var removed = drafts.RemoveAll(draft => draft.Id == id);
            if (removed == 0)
            {
                return false;
            }

            Write(drafts);
            return true;
        }
    }

    public static IReadOnlyList<PosCopiedLine>? Lines(Guid id)
    {
        var draft = Find(id);
        if (draft is null)
        {
            return null;
        }

        return draft.Lines.Select(line => new PosCopiedLine(
            line.ArticleId,
            line.Code,
            line.Designation,
            line.Quantity,
            line.Price,
            line.Discount,
            line.VatRateId,
            line.VatPercentage)).ToList();
    }

    public static IReadOnlyList<PosDocumentRow> Merge(IReadOnlyList<PosDocumentRow> remote, DateTime start, DateTime end)
    {
        var from = start.Date;
        var to = end.Date.AddDays(1);
        List<StoredDraft> drafts;
        lock (Gate)
        {
            drafts = Read().Where(draft => draft.CreatedAt >= from && draft.CreatedAt < to).ToList();
        }

        if (drafts.Count == 0)
        {
            return remote;
        }

        var localIds = drafts.Select(draft => draft.Id).ToHashSet();
        var localRows = drafts.Select(draft => new PosDocumentRow
        {
            Id = draft.Id,
            DocumentDate = draft.CreatedAt,
            Number = string.IsNullOrWhiteSpace(draft.Acronym) ? "Rascunho" : draft.Acronym + " rascunho",
            Status = "Rascunho",
            EntityName = draft.CustomerName,
            TaxId = draft.FiscalNumber,
            FinalTotal = draft.Total,
            PaidAmount = 0,
            DebitAmount = draft.Total,
            AssociatedDocuments = string.Empty
        });
        return localRows
            .Concat(remote.Where(row => localIds.Contains(row.Id) == false))
            .OrderByDescending(row => row.DocumentDate)
            .ToList();
    }

    private static List<StoredDraft> Read()
    {
        var path = FilePath();
        if (File.Exists(path) == false)
        {
            return [];
        }

        try
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<StoredDraft>>(json, Json) ?? [];
        }
        catch (Exception)
        {
            return [];
        }
    }

    private static void Write(List<StoredDraft> drafts)
    {
        var path = FilePath();
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, JsonSerializer.Serialize(drafts, Json));
    }

    private static string FilePath()
    {
        return Path.Combine(AppContext.BaseDirectory, "local-drafts.json");
    }
}

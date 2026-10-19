namespace LogicPOS.Core.BackOffice;

public sealed class ListingColumn
{
    public required string Key { get; init; }

    public required string Header { get; init; }

    public bool Visible { get; init; } = true;
}

public sealed class ListingRow
{
    public Guid Id { get; init; }

    public bool CanDelete { get; init; }

    public Dictionary<string, string> Values { get; init; } = new();

    public string this[string key] => Values.TryGetValue(key, out var value) ? value : string.Empty;
}

public sealed class ListingAction
{
    public required string Key { get; init; }

    public required string Label { get; init; }
}

public sealed class ListingField
{
    public required string Key { get; init; }

    public required string Label { get; init; }

    public required string Kind { get; init; }

    public string Value { get; set; } = string.Empty;

    public string? Lookup { get; init; }

    public bool ReadOnly { get; init; }

    public bool Required { get; init; }

    public string? Group { get; init; }

    public string? ShowWhen { get; init; }

    /// <summary>Fixed values for Kind "choice".</summary>
    public IReadOnlyList<string>? Options { get; init; }
}

public sealed class ListingSnapshot
{
    public string Title { get; init; } = string.Empty;

    public bool CanCreate { get; init; }

    public bool CanEdit { get; init; }

    public bool CanDelete { get; init; }

    public IReadOnlyList<ListingColumn> Columns { get; init; } = [];

    public IReadOnlyList<ListingRow> Rows { get; init; } = [];

    public IReadOnlyList<ListingAction> Actions { get; init; } = [];

    public IReadOnlyList<string> Modes { get; init; } = [];

    public string? Notice { get; init; }
}

public sealed class ListingSaveResult
{
    public bool Succeeded { get; init; }

    public string? Error { get; init; }

    public string? Message { get; init; }

    /// <summary>
    /// Extra notice for the UI (e.g. SAF-T validation issues + helpdesk link).
    /// </summary>
    public string? Detail { get; init; }

    /// <summary>
    /// When true, <see cref="Message"/> is a temp path and the Avalonia UI must ask where to save.
    /// </summary>
    public bool NeedsSaveAs { get; init; }

    public Guid Id { get; init; }

    public IReadOnlyList<Guid> Ids { get; init; } = [];

    public static ListingSaveResult Ok(Guid id, string? message = null, string? detail = null, bool needsSaveAs = false) => new()
    {
        Succeeded = true,
        Id = id,
        Message = message,
        Detail = detail,
        NeedsSaveAs = needsSaveAs,
        Ids = id == Guid.Empty ? [] : [id]
    };

    public static ListingSaveResult OkMany(IReadOnlyList<Guid> ids, string? message = null) => new()
    {
        Succeeded = true,
        Message = message,
        Ids = ids
    };

    public static ListingSaveResult Fail(string error) => new()
    {
        Succeeded = false,
        Error = error
    };
}

public static class LookupChoice
{
    /// <summary>
    /// Uses an explicit selection. A typed value is accepted only when it equals one label;
    /// a fragment of the label or search text is never chosen on its own.
    /// </summary>
    public static LookupOption? Choose(LookupOption? selectedLookup, LookupOption? selectedItem, string? text, System.Collections.IEnumerable? items)
    {
        if (selectedLookup is { Id: var selectedId } && selectedId != Guid.Empty)
        {
            return selectedLookup;
        }

        if (selectedItem is { Id: var itemId } && itemId != Guid.Empty)
        {
            return selectedItem;
        }

        text = (text ?? string.Empty).Trim();
        if (text.Length == 0 || items is null)
        {
            return null;
        }

        LookupOption? match = null;
        foreach (var item in items)
        {
            if (item is not LookupOption candidate || candidate.Id == Guid.Empty)
            {
                continue;
            }

            if (string.Equals(candidate.Label, text, StringComparison.CurrentCultureIgnoreCase) == false)
            {
                continue;
            }

            if (match is not null)
            {
                return null;
            }

            match = candidate;
        }

        return match;
    }
}

public sealed class LookupOption
{
    public Guid Id { get; init; }

    public string Label { get; init; } = string.Empty;

    public string? Meta { get; init; }

    public string? Search { get; init; }

    public override string ToString() => Label;

    public static LookupOption FromArticle(Guid id, string? code, string? designation, string? barcode)
    {
        var number = (code ?? string.Empty).Trim();
        var name = (designation ?? string.Empty).Trim();
        var label = number.Length == 0
            ? name
            : name.Length == 0 ? number : number + " - " + name;
        if (label.Length == 0)
        {
            label = id.ToString();
        }

        var parts = new[] { number, name, (barcode ?? string.Empty).Trim() };
        return new LookupOption
        {
            Id = id,
            Label = label,
            Search = string.Join(' ', parts.Where(part => part.Length > 0))
        };
    }
}

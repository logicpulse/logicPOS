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

    public string Value { get; init; } = string.Empty;

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

    public Guid Id { get; init; }

    public IReadOnlyList<Guid> Ids { get; init; } = [];

    public static ListingSaveResult Ok(Guid id, string? message = null) => new()
    {
        Succeeded = true,
        Id = id,
        Message = message,
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

public sealed class LookupOption
{
    public Guid Id { get; init; }

    public string Label { get; init; } = string.Empty;

    public string? Meta { get; init; }

    public override string ToString() => Label;
}

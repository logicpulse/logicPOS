namespace LogicPOS.Core.FrontOffice;

public sealed class PosMenuItem
{
    public PosMenuItem(Guid id, Guid parentId, string text, bool favorite, string? imageBase64 = null)
    {
        Id = id;
        ParentId = parentId;
        Text = text;
        Favorite = favorite;
        ImageBase64 = imageBase64;
    }

    public Guid Id { get; }

    public Guid ParentId { get; }

    public string Text { get; }

    public bool Favorite { get; }

    /// <summary>Optional button image from seed/DB (base64, no data: prefix).</summary>
    public string? ImageBase64 { get; }
}

public sealed class PosCatalog
{
    public PosCatalog(
        IReadOnlyList<PosMenuItem> families,
        IReadOnlyList<PosMenuItem> subfamilies,
        IReadOnlyList<PosArticle> articles)
    {
        Families = families;
        Subfamilies = subfamilies;
        Articles = articles;
    }

    public IReadOnlyList<PosMenuItem> Families { get; }

    public IReadOnlyList<PosMenuItem> Subfamilies { get; }

    public IReadOnlyList<PosArticle> Articles { get; }

    public static PosCatalog Empty { get; } = new(Array.Empty<PosMenuItem>(), Array.Empty<PosMenuItem>(), Array.Empty<PosArticle>());
}

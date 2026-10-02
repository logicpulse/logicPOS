namespace LogicPOS.Core.FrontOffice;

public sealed class PosMenuItem
{
    public PosMenuItem(Guid id, Guid parentId, string text, bool favorite)
    {
        Id = id;
        ParentId = parentId;
        Text = text;
        Favorite = favorite;
    }

    public Guid Id { get; }

    public Guid ParentId { get; }

    public string Text { get; }

    public bool Favorite { get; }
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

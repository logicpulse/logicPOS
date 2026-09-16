namespace LogicPOS.ApiServer.DTOs;

public sealed class ArticleTypeResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public uint Order { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public bool HasPrice { get; set; }
}

public sealed class AddArticleTypeRequest
{
    public string Designation { get; set; } = string.Empty;
    public bool HasPrice { get; set; }
    public string? Notes { get; set; }
}

public sealed class UpdateArticleTypeRequest
{
    public uint Order { get; set; }
    public string? Code { get; set; }
    public string Designation { get; set; } = string.Empty;
    public bool HasPrice { get; set; }
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; }
}

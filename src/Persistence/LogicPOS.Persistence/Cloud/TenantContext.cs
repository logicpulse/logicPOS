namespace LogicPOS.Persistence.Cloud;

/// <summary>
/// Current tenant for the request or background scope.
/// The value is also stored in <see cref="AsyncLocal{T}"/> so singleton services
/// (license overlay) can read it on the same execution flow.
/// </summary>
public sealed class TenantContext
{
    private static readonly AsyncLocal<string?> Ambient = new();

    public static string? AmbientClientId => Ambient.Value;

    public string? ClientId
    {
        get => Ambient.Value;
        set => Ambient.Value = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

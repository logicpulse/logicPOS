namespace LogicPOS.ApiServer.Authentication;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "LogicPOS.ApiServer";
    public string Audience { get; set; } = "LogicPOS.Client";
    public string SigningKey { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 480;
}

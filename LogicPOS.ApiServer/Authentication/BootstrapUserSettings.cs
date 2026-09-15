namespace LogicPOS.ApiServer.Authentication;

public sealed class BootstrapUserSettings
{
    public const string SectionName = "BootstrapUser";

    public Guid UserId { get; set; }
    public Guid TerminalId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;

    public bool IsConfigured =>
        UserId != Guid.Empty &&
        string.IsNullOrWhiteSpace(Username) == false &&
        string.IsNullOrWhiteSpace(Pin) == false;
}

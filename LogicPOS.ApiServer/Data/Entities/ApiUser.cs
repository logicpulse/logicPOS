namespace LogicPOS.ApiServer.Data.Entities;

public sealed class ApiUser
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}

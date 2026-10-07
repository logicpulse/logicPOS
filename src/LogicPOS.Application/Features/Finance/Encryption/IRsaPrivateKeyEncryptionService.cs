namespace LogicPOS.Application.Features.Finance.Encryption;

public interface IRsaPrivateKeyEncryptionService
{
    public string SignWithAgtFeSoftwarePrivateKey(string plainText);
    public Task<string> SignWithAgtFeContributorPrivateKeyAsync(string plainText, CancellationToken ct = default);

    public string SignWithSaftPrivateKey(string plainText);
}

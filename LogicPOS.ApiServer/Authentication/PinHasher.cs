using System.Security.Cryptography;

namespace LogicPOS.ApiServer.Authentication;

public sealed class PinHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    public (string Hash, string Salt) Hash(string pin)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(pin, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public bool Verify(string pin, string expectedHash, string expectedSalt)
    {
        if (string.IsNullOrWhiteSpace(expectedHash) || string.IsNullOrWhiteSpace(expectedSalt))
        {
            return false;
        }

        try
        {
            var salt = Convert.FromBase64String(expectedSalt);
            var hash = Rfc2898DeriveBytes.Pbkdf2(pin, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
            return CryptographicOperations.FixedTimeEquals(hash, Convert.FromBase64String(expectedHash));
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

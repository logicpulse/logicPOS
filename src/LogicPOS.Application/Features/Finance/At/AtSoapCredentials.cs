namespace LogicPOS.Application.Features.Finance.At;

public sealed record AtSoapCredentials(string Username,
                                 string EncryptedPassword,
                                 string EncryptedCreated,
                                 string Nonce);
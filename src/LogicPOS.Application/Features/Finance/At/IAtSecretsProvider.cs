using System;
using System.Security.Cryptography.X509Certificates;

namespace LogicPOS.Application.Features.Finance.At;

public interface IAtSecretsProvider
{
    public X509Certificate2 GetPublicKeyCertificate();
    public X509Certificate2 GetCommunicationCertificate();
    public Task<(string Username, string Password)> GetUsernameAndPasswordAsync();
    public string GetSeriesUrl();
    public string GetTransportDocumentsUrl();
    public string GetAgriculturalUrl();
    public string GetInvoicesUrl();
    public string GetTransportDocumentsAction();
    public string GetInvoicesAction();
    public string GetSeriesAction();
}

namespace LogicPOS.Application.Features.Finance.Agt.Services;

public interface IAgtSecretsProvider
{
    public string HttpClientUsername { get; } 
    public string HttpClientPassword { get; }
    public string RegisterInvoiceEndpoint { get; }
    public string GetInvoiceStatusEndpoint { get; }
    public string ListInvoicesEndpoint { get; }
    public string GetInvoiceEndpoint { get; }
    public string RequestSeriesEndpoint { get; }
    public string ListSeriesEndpoint { get; }
    public string ValidateDocumentEndpoint { get; }
    public string GetGetContributorEndpoint(string documentType, string documentNumber);
}
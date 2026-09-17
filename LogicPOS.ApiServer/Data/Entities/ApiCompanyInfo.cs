namespace LogicPOS.ApiServer.Data.Entities;

public sealed class ApiCompanyInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string CommercialName { get; set; } = string.Empty;
    public string LogoPng { get; set; } = string.Empty;
    public string LogoBmp { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string CountryCode2 { get; set; } = "PT";
    public string Phone { get; set; } = string.Empty;
    public string MobilePhone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string FiscalNumber { get; set; } = string.Empty;
    public string StockCapital { get; set; } = string.Empty;
    public string DocumentFinalLine1 { get; set; } = string.Empty;
    public string DocumentFinalLine2 { get; set; } = string.Empty;
    public string TaxEntity { get; set; } = string.Empty;
    public string Fax { get; set; } = string.Empty;
    public string TicketFinalLine1 { get; set; } = string.Empty;
    public string TicketFinalLine2 { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = "EUR";
    public string AgtLogo { get; set; } = string.Empty;
}

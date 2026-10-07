using LogicPOS.Shared.Features.Software;

namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public struct SaftSoftwareInformation
{
    public string Name { get; set; }
    public string Company { get; set; }
    public string CompanyPhone { get; set; } 
    public string CompanyEmail { get; set; }
    public string CompanyWeb { get; set; } 
    public string ProductCompanyTaxId { get; set; }
    public string CertificateNumber { get; set; } 
    public string ProductId { get; set; } 
    public string ProductVersion { get; set; }
    public string SaftVersion { get; set; } 
    public string SaftVersionPrefix { get; set; } 


    public static SaftSoftwareInformation Angola => new SaftSoftwareInformation
    {
        Name = SoftwareInfo.Angola.Name,
        Company = SoftwareInfo.Angola.Company,
        CompanyPhone = SoftwareInfo.Angola.CompanyPhone,
        CompanyEmail = SoftwareInfo.Angola.CompanyEmail,
        CompanyWeb = SoftwareInfo.Angola.CompanyWeb,
        ProductCompanyTaxId = SoftwareInfo.Angola.ProductCompanyTaxId,
        CertificateNumber = SoftwareInfo.Angola.SaftCertificateNumber,
        ProductId = SoftwareInfo.Angola.ProductId,
        ProductVersion = "v1.0",
        SaftVersion = "1.01_01",
        SaftVersionPrefix = "AO"
    };

    public static SaftSoftwareInformation Portugal => new SaftSoftwareInformation
    {
        Name = SoftwareInfo.Portugal.Name,
        Company = SoftwareInfo.Portugal.Company,
        CompanyPhone = SoftwareInfo.Portugal.CompanyPhone,
        CompanyEmail = SoftwareInfo.Portugal.CompanyEmail,
        CompanyWeb = SoftwareInfo.Portugal.CompanyWeb,
        ProductCompanyTaxId = SoftwareInfo.Portugal.ProductCompanyTaxId,
        CertificateNumber = SoftwareInfo.Portugal.SaftCertificateNumber,
        ProductId = SoftwareInfo.Portugal.ProductId,
        ProductVersion = "v1.4.200",
        SaftVersion = "1.04_01",
        SaftVersionPrefix = "PT"
    };
}
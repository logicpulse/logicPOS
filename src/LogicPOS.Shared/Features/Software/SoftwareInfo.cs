namespace LogicPOS.Shared.Features.Software;

public record SoftwareInfo
{
    public string Name { get; set; } = null!;
    public string Company { get; set; } = null!;
    public string CompanyPhone { get; set; } = null!;
    public string CompanyEmail { get; set; } = null!;
    public string CompanyWeb { get; set; } = null!;
    public string ProductCompanyTaxId { get; set; } = null!;
    public string SaftCertificateNumber { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public string ProductVersion { get; set; } = null!;
    
    public static readonly SoftwareInfo Angola = new SoftwareInfo
    {
        Name = "LogicPOS",
        Company = "LogicPulse Technologies",
        CompanyPhone = "+351 233 042 347 / +351 910 287 029 / +351 800 180 500",
        CompanyEmail = "comercial@logicpulse.com",
        CompanyWeb = "http://www.logicpulse.com",
        ProductCompanyTaxId = "5171164380",
        SaftCertificateNumber = "221/AGT/2019",
        ProductId = "LogicPOS/LOGICPULSE ANGOLA",
        ProductVersion = "v1.0",
    };

    public static readonly SoftwareInfo Portugal = new SoftwareInfo
    {
        Name = "LogicPos",
        Company = "LogicPulse Technologies",
        CompanyPhone = "+351 233 042 347 / +351 910 287 029 / +351 800 180 500",
        CompanyEmail = "comercial@logicpulse.com",
        CompanyWeb = "http://www.logicpulse.com",
        ProductCompanyTaxId = "508278155",
        ProductId = "LogicPos/Logicpulse",
        SaftCertificateNumber = "2543",
        ProductVersion = "v1.4.200"
    };
}
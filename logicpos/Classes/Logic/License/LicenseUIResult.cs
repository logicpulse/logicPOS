using logicpos.Classes.Enums.Dialogs;

namespace logicpos.Classes.Logic.License
{
    public class LicenseUIResult
    {
        public LicenseUIResponse Response { get; set; }
        public string Name { get; set; }

        public string Company { get; set; }
        public string FiscalNumber { get; set; }

        public string Address { get; set; }
        public string Email { get; set; }

        public string Phone { get; set; }

        public LicenseUIResult(LicenseUIResponse pResponse)
        {
            Response = pResponse;
        }

        public LicenseUIResult(LicenseUIResponse pResponse, string pName, string pCompany, string pFiscalNumber, string pAddress, string pEmail, string pPhone)
        {
            Response = pResponse;
            Name = pName;
            Company = pCompany;
            FiscalNumber = pFiscalNumber;
            Address = pAddress;
            Email = pEmail;
            Phone = pPhone;
        }
    }
}

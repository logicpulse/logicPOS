using logicpos.Classes.Enums.Dialogs;

namespace logicpos.Classes.Logic.License
{
    public class LicenseUIResult
    {
        private LicenseUIResponse _response;
        public LicenseUIResponse Response
        {
            get { return _response; }
            set { _response = value; }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private string _company;
        public string Company
        {
            get { return _company; }
            set { _company = value; }
        }
        private string _fiscalNumber;
        public string FiscalNumber
        {
            get { return _fiscalNumber; }
            set { _fiscalNumber = value; }
        }
        private string _address;
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }
        private string _email;
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
        private string _phone;
        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        public LicenseUIResult(LicenseUIResponse pResponse)
        {
            _response = pResponse;
        }

        public LicenseUIResult(LicenseUIResponse pResponse, string pName, string pCompany, string pFiscalNumber, string pAddress, string pEmail, string pPhone)
        {
            _response = pResponse;
            _name = pName;
            _company = pCompany;
            _fiscalNumber = pFiscalNumber;
            _address = pAddress;
            _email = pEmail;
            _phone = pPhone;
        }
    }
}

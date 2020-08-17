using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.RegularExpressions;
//IN:009268 VAT INFO CHECKER AND AUTO-COMPLETE FIELDS
namespace logicpos
{   
    public class EuropeanVatInformation
    {
		//Limpa caracteres inválidos que chegam do WebService
        static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[*/ƒ]", "",
                                     RegexOptions.Compiled, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters, 
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }

        private EuropeanVatInformation() { }

        public string CountryCode { get; private set; }
        public string VatNumber { get; private set; }
        public string Address { get; private set; }
        public string Name { get; private set; }
        public override string ToString() => CountryCode + " " + VatNumber + ": " + Name + ", " + Address.Replace("\n", ", ");

        public static EuropeanVatInformation Get(string countryCodeAndVatNumber)
        {
            if (countryCodeAndVatNumber == null)
                throw new ArgumentNullException(nameof(countryCodeAndVatNumber));

            if (countryCodeAndVatNumber.Length < 3)
                return null;

            return Get(countryCodeAndVatNumber.Substring(0, 2), countryCodeAndVatNumber.Substring(2));
        }

        public static EuropeanVatInformation Get(string countryCode, string vatNumber)
        {
            if (countryCode == null)
                throw new ArgumentNullException(nameof(countryCode));

            if (vatNumber == null)
                throw new ArgumentNullException(nameof(vatNumber));

            countryCode = countryCode.Trim();
            vatNumber = vatNumber.Trim().Replace(" ", string.Empty);

            const string url = "http://ec.europa.eu/taxation_customs/vies/services/checkVatService";
            const string xml = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:ec.europa.eu:taxud:vies:services:checkVat:types"">
    <soapenv:Header/>
    <soapenv:Body>
      <urn:checkVat>
         <urn:countryCode>{0}</urn:countryCode>
         <urn:vatNumber>{1}</urn:vatNumber>
      </urn:checkVat>
    </soapenv:Body>
    </soapenv:Envelope>";

            try
            {
                using (var client = new WebClient())
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(client.UploadString(url, string.Format(xml, countryCode, vatNumber)));
                    var response = doc.SelectSingleNode("//*[local-name()='checkVatResponse']") as XmlElement;
                    if (response == null || response["valid"]?.InnerText != "true")
                        return null;

                    var info = new EuropeanVatInformation();
                    
                    info.CountryCode = CleanInput(response["countryCode"].InnerText);
                    info.VatNumber = CleanInput(response["vatNumber"].InnerText);
					//IN009346 - Correção - Update de Info via NIF para União Europeia
                    info.Name = CleanInput(response["name"].InnerText);
                    info.Name = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(info.Name.ToLower());

                    info.Address = CleanInput(response["address"].InnerText);
                    info.Address = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(info.Address.ToLower());
                    return info;
                }
            }
            catch
            {
                return null;
            }
        }
    }


}

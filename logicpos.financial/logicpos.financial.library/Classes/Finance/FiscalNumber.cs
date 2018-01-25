using System;

namespace logicpos.financial.library.Classes.Finance
{
    //http://info.portaldasfinancas.gov.pt/NR/rdonlyres/D1DFFB13-B9F1-49C3-BC0C-9DDB770367EE/0/Decreto-Lei_14-2013.pdf
    //1 ou 2 (pessoa singular)
    //5 (pessoa colectiva)
    //6 (pessoa colectiva pública)
    //8 (empresário em nome individual)
    //9 (pessoa colectiva irregular ou número provisório)

    public class FiscalNumber
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Test With
        //FiscalNumber.IsValidFiscalNumber("503218820", "PT");
        //FiscalNumber.IsValidFiscalNumber("502757191", "PT");
        //FiscalNumber.IsValidFiscalNumber("503218103", "PT");
        //FiscalNumber.IsValidFiscalNumber("503242233", "PT");
        //FiscalNumber.IsValidFiscalNumber("503242217", "PT");
        //FiscalNumber.IsValidFiscalNumber("182692124", "PT");
        //FiscalNumber.IsValidFiscalNumber("PT503242217", "PT");
        //FiscalNumber.IsValidFiscalNumber("PT182692124", "PT");

        //TODO: Muga: Will be Changed/Replaced by Muga
        public static bool IsValidFiscalNumber(string pFiscalNumber, string pCountryCode2)
        {
            bool result = false;

            try
            {
                //Get FiscalNumber without CountryCode2 
                string fiscalNumber = ExtractFiscalNumber(pFiscalNumber, pCountryCode2);

                switch (pCountryCode2)
                {
                    case "PT":

                        if (fiscalNumber.Length == 9)
                        {
                            int[] s = new int[9];
                            int r = 0;
                            int sum = 0;
                            int checkDigit = 0;

                            s[0] = Convert.ToInt16(fiscalNumber.Substring(0, 1));
                            s[1] = Convert.ToInt16(fiscalNumber.Substring(1, 1));
                            s[2] = Convert.ToInt16(fiscalNumber.Substring(2, 1));
                            s[3] = Convert.ToInt16(fiscalNumber.Substring(3, 1));
                            s[4] = Convert.ToInt16(fiscalNumber.Substring(4, 1));
                            s[5] = Convert.ToInt16(fiscalNumber.Substring(5, 1));
                            s[6] = Convert.ToInt16(fiscalNumber.Substring(6, 1));
                            s[7] = Convert.ToInt16(fiscalNumber.Substring(7, 1));
                            s[8] = Convert.ToInt16(fiscalNumber.Substring(8, 1));

                            sum = (9 * s[0] + 8 * s[1] + 7 * s[2] + 6 * s[3] + 5 * s[4] + 4 * s[5] + 3 * s[6] + 2 * s[7]);

                            r = 11 - (sum % 11);
                            if (r == 10 || r == 11) r = 0;
                            checkDigit = r;
                            if (checkDigit == s[8]) result = true;
                            //_log.Debug(string.Format("FiscalNumber[{0}]:, sum: [{1}], checkDigit: [{2}], result:[{3}] ", pFiscalNumber, sum, checkDigit, result));
                        }
                        break;

                    default:
                        //Always result true if Country is not Implemented in Switch
                        result = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //NIF das Empresas
        //http://www.nif.pt/nif-das-empresas/
        public static bool IsSingularEntity(string pFiscalNumber, string pCountryCode2)
        {
            //Allways result true if County is not Implemented in Switch
            bool result = true;

            string fiscalNumber = ExtractFiscalNumber(pFiscalNumber, pCountryCode2);

            if (pFiscalNumber != string.Empty)
            {
                switch (pCountryCode2)
                {
                    case "PT":
                        //Start with 1 or 2 is a Singular Entity
                        result = (Convert.ToString(fiscalNumber[0]) == "1" || Convert.ToString(fiscalNumber[0]) == "2");
                        break;
                    default:
                        break;
                }            
            }
            return result;
        }

        public static string ExtractFiscalNumber(string pFiscalNumber, string pCountryCode2)
        {
            string result = string.Empty;

            try
            {
                switch (pCountryCode2)
                {
                    case "PT":
                        if (pFiscalNumber.Length == 9 || pFiscalNumber.Length == 11)
                        {
                            result = (pFiscalNumber.Substring(0, 2) == "PT") 
                                ? pFiscalNumber.Substring(2, pFiscalNumber.Length - 2) 
                                : pFiscalNumber;
                        }
                        else
                        {
                            result = String.Empty;
                        }
                        break;

                    default:
                        if (pFiscalNumber.StartsWith(pCountryCode2)) {
                            result = pFiscalNumber.Substring(pCountryCode2.Length, pFiscalNumber.Length - pCountryCode2.Length);
                        }
                        else
                        {
                            result = pFiscalNumber;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //Detect if first 2 chars is a CountryCode 2 if so, extract it
        public static string ExtractFiscalNumber(string pFiscalNumber)
        {
            string result = string.Empty;

            int intPrefix;
            int intPrefixLength = 2;
            string fiscalNumberPrefix = pFiscalNumber.Substring(0, intPrefixLength);
            bool isNumeric = int.TryParse(fiscalNumberPrefix, out intPrefix);

            result = (isNumeric) 
                ? pFiscalNumber 
                : pFiscalNumber.Substring(intPrefixLength, pFiscalNumber.Length - 2)
            ;

            return result;
        }
    }
}

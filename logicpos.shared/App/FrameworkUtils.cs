using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.resources.Resources.Localization;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Classes.Orders;
using logicpos.shared.Classes.Others;
using logicpos.shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.ServiceModel.Configuration;
using System.Text;
using System.Text.RegularExpressions;

namespace logicpos.shared.App
{
    public class FrameworkUtils : logicpos.datalayer.App.FrameworkUtils
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Strings

        public static string RemoveCarriageReturn(string pInput)
        {
            return pInput.Replace("\r\n", string.Empty);
        }

        public static string RemoveExtraWhiteSpaces(string pInput)
        {
            return Regex.Replace(pInput, @"\s+", " ");
        }

        public static string RemoveCarriageReturnAndExtraWhiteSpaces(string pInput)
        {
            string result = string.Empty;
            result = RemoveCarriageReturn(RemoveExtraWhiteSpaces(pInput));

            //Remove Initial Space if Exists
            if (result.StartsWith(" "))
            {
                result = result.Substring(1, result.Length - 1);
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //DateTime

        public static String CurrentDateTime()
        {
            return CurrentDateTime(SettingsApp.DateTimeFormat);
        }

        public static String CurrentDateTime(String pDateTimeFormat)
        {
            return CurrentDateTimeAtomic().ToString(pDateTimeFormat, System.Globalization.CultureInfo.GetCultureInfo(GlobalFramework.CurrentCulture.Name));
            //return CurrentDateTimeAtomic().ToString("F", System.Globalization.CultureInfo.GetCultureInfo(GlobalFramework.CurrentCulture.Name));
        }

        public static DateTime CurrentDateTimeAtomicMidnight()
        {
            return DateTimeToMidnightDate(CurrentDateTimeAtomic());
        }

        public static DateTime DateTimeToMidnightDate(DateTime pDateTime)
        {
            DateTime result = new DateTime(pDateTime.Year, pDateTime.Month, pDateTime.Day);

            return result;
        }

        public static string DateTimeToCombinedDateTimeString(object pValue)
        {
            DateTime tmpData = Convert.ToDateTime(pValue);
            string result = "" + tmpData.ToString("" + SettingsApp.DateTimeFormatCombinedDateTime);
            return (result);
        }

        public static string DateToString(object pValue)
        {
            DateTime tmpData = Convert.ToDateTime(pValue);
            string result = "" + tmpData.ToString("" + SettingsApp.DateTimeFormatDocumentDate);
            return (result);
        }

        public static string DateToString(DateTime pValue)
        {
            string result = string.Empty;

            try
            {
                result = pValue.ToString(SettingsApp.DateTimeFormatDocumentDate);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        public static string DateTimeToString(DateTime pValue)
        {
            string result = string.Empty;

            try
            {
                result = pValue.ToString(SettingsApp.DateTimeFormat);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //DateTime - Utils Days and Holidays

        //Get Holidays DateTime List From  ConfigurationHolidays Collection
        public static Dictionary<DateTime, bool> GetHolidays()
        {
            if (holidaysDic == null)
                holidaysDic= GetHolidays(DateTime.Now.Year);

            return holidaysDic;
        }

        static Dictionary<DateTime, bool> holidaysDic;

        public static Dictionary<DateTime, bool> GetHolidays(int pYear)
        {
            bool debug = false;
            DateTime currentDateTime;
            Dictionary<DateTime, bool> result = new Dictionary<DateTime, bool>();
            CriteriaOperator criteriaOperator = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled is NULL) AND (Year = 0 OR Year = {0})", pYear));
            SortingCollection sortingCollection = new SortingCollection();
            sortingCollection.Add(new SortProperty("Ord", DevExpress.Xpo.DB.SortingDirection.Ascending));
            XPCollection xpcConfigurationHolidays = GetXPCollectionFromCriteria(GlobalFramework.SessionXpo, typeof(cfg_configurationholidays), criteriaOperator, sortingCollection);

            if (xpcConfigurationHolidays.Count > 0)
            {
                foreach (cfg_configurationholidays item in xpcConfigurationHolidays)
                {
                    currentDateTime = new DateTime(pYear, item.Month, item.Day);
                    result.Add(currentDateTime, item.Fixed);
                    if (debug) _log.Debug(string.Format("DayOfWeek: [{0}:{1}:{2}:{3}]", currentDateTime.ToString(SettingsApp.DateFormat), GlobalFramework.CurrentCulture.DateTimeFormat.DayNames[(int)currentDateTime.DayOfWeek], item.Fixed, IsHoliday(currentDateTime)));
                }
            }
            return result;
        }

        public static bool IsHoliday(DateTime pDateTime)
        {
            return IsHoliday(GetHolidays(), DateTimeToMidnightDate(pDateTime));
        }

        public static bool IsHoliday(Dictionary<DateTime, bool> pHolidays, DateTime pDateTime)
        {
            bool result = false;

            foreach (var item in pHolidays)
            {
                //Fixed
                if (item.Value)
                {
                    if (item.Key.Month == pDateTime.Month && item.Key.Day == pDateTime.Day)
                        result = true;
                }
                else
                {
                    if (item.Key.Year == pDateTime.Year && item.Key.Month == pDateTime.Month && item.Key.Day == pDateTime.Day)
                        result = true;
                }
            }
            return result;
        }

        //From Date to Now
        public static List<DateTime> GetUtilDays(DateTime pDateStart)
        {
            return GetUtilDays(pDateStart, CurrentDateTimeAtomicMidnight());
        }

        public static List<DateTime> GetUtilDays(DateTime pDateStart, DateTime pDateEnd)
        {
            return GetUtilDays(DateTimeToMidnightDate(pDateStart), DateTimeToMidnightDate(pDateEnd), false);
        }

        public static List<DateTime> GetUtilDays(DateTime pDateStart, bool pWithHoydays)
        {
            return GetUtilDays(DateTimeToMidnightDate(pDateStart), CurrentDateTimeAtomicMidnight(), pWithHoydays);
        }

        public static List<DateTime> GetUtilDays(DateTime pDateStart, DateTime pDateEnd, bool pWithHoydays)
        {
            bool debug = false;
            string isHoliday = String.Empty;
            List<DateTime> result = new List<DateTime>();
            //Range Interval
            DateTime startDateTime = pDateStart.Date.AddDays(1);

            while (startDateTime < pDateEnd.Date)
            {
                if (startDateTime.DayOfWeek != DayOfWeek.Saturday && startDateTime.DayOfWeek != DayOfWeek.Sunday)
                {
                    isHoliday = (IsHoliday(startDateTime)) ? "Holiday" : String.Empty;
                    if (debug) _log.Debug(string.Format("DayOfWeek: [{0}:{1}:{2}]", startDateTime.ToString(SettingsApp.DateFormat), GlobalFramework.CurrentCulture.DateTimeFormat.DayNames[(int)startDateTime.DayOfWeek], isHoliday));

                    if ((pWithHoydays && !IsHoliday(startDateTime)) || !pWithHoydays)
                    {
                        result.Add(startDateTime);
                    }
                }
                //Advance onde Day
                startDateTime = startDateTime.AddDays(1);
            }
            return result;
        }

        //Returns DateTime Back x UtilDays (Returns the Date x util days back, without Holidays and WeekEnd Days)
        public static DateTime GetDateTimeBackUtilDays(DateTime pDateTime, int pDays, bool pWithHoydays)
        {
            bool debug = false;
            //Remove CurrentDay
            DateTime result = DateTimeToMidnightDate(pDateTime);
            string isHoliday = String.Empty;
            string isWeekEnd = String.Empty;

            int i = 0;
            while (i < pDays)
            {
                //Start Back one Day
                result = result.AddDays(-1);

                if (result.DayOfWeek != DayOfWeek.Saturday && result.DayOfWeek != DayOfWeek.Sunday)
                {
                    isWeekEnd = String.Empty;
                    isHoliday = (IsHoliday(result)) ? "<Holiday>" : String.Empty;

                    if ((pWithHoydays && !IsHoliday(result)) || !pWithHoydays)
                    {
                        i++;
                    }
                }
                else
                {
                    isWeekEnd = "<WeekEnd>";
                }
                if (debug) _log.Debug(string.Format("DayOfWeek: [{0}:{1}:{2}{3}{4}]", i.ToString("000"), result.ToString(SettingsApp.DateFormat), GlobalFramework.CurrentCulture.DateTimeFormat.DayNames[((int)result.DayOfWeek)], isWeekEnd, isHoliday));
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Prices & Discounts

        /* REMOVED Now use PriceProperties Helper Object

        public static decimal CalcPrice(decimal pPrice, decimal pDiscount)
        {
          return CalcDiscount(pPrice, pDiscount);
        }

        public static decimal CalcPrice(decimal pPrice, decimal pDiscount, decimal pVat)
        {
          decimal result = CalcDiscount(pPrice, pDiscount);

          result = result * ((pVat / 100) + 1);

          return (result);
        }

        public static decimal CalcDiscount(decimal pPrice, decimal pDiscount)
        {
          return pPrice - ((pPrice * pDiscount) / 100);
        }

        public static decimal CalcPriceReverse(decimal pFinalPrice, decimal pDiscount, decimal pVat, bool pPriceWithVat)
        {
          decimal priceWithoutVat = pFinalPrice;

          //if (pPriceWithVat)
          //{
          priceWithoutVat = priceWithoutVat / (pVat / 100 + 1);
          //}

          decimal discountHelper = 1 - (pDiscount / 100);
          return priceWithoutVat / discountHelper;
        }
        */

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Prices & Discounts : Article

        /// <summary>
        /// Used to Get Global Discount
        /// </summary>
        /// <returns></returns>
        public static decimal GetDiscountGlobal()
        {
            decimal result = 0.0m;
            try
            {
                if (GlobalFramework.SessionApp != null
                    && GlobalFramework.SessionApp.OrdersMain.ContainsKey(GlobalFramework.SessionApp.CurrentOrderMainOid)
                )
                {
                    //get CurrentOrderMain
                    OrderMain orderMain = GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid];
                    //Get Table to Get Discount
                    pos_configurationplacetable xConfigurationPlaceTable = (pos_configurationplacetable)GetXPGuidObject(GlobalFramework.SessionXpo, typeof(pos_configurationplacetable), orderMain.Table.Oid);
                    //Get Fresh Discount From Table/Future 
                    if (xConfigurationPlaceTable != null)
                    {
                        result = xConfigurationPlaceTable.Discount;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            return result;
        }

        public static PriceProperties GetArticlePrice(fin_article pArticle, TaxSellType pTaxSellType)
        {
            //get PriceType from CurrentOrderMain Table
            OrderMain orderMain = GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid];
            return GetArticlePrice(pArticle, orderMain.Table.PriceType, pTaxSellType);
        }

        public static PriceProperties GetArticlePrice(fin_article pArticle, PriceType pPriceType, TaxSellType pTaxSellType)
        {
            decimal priceSource = 0.0m;
            decimal priceDefault = 0.0m;
            decimal priceTax = 0.0m;

            // Get priceTax Based on AppOperationMode : in retail mode VatOnTable is always null
            if (SettingsApp.AppMode == AppOperationMode.Default && !GlobalFramework.AppUseBackOfficeMode)
            {
                // Default : Restaurants with dual Tax ex Normal, TakeAway
                priceTax = (pTaxSellType == TaxSellType.Normal) ? pArticle.VatOnTable.Value : pArticle.VatDirectSelling.Value;
            }
            else if (SettingsApp.AppMode == AppOperationMode.Retail)
            {
                // Mono priceTax 
                priceTax = pArticle.VatDirectSelling.Value;
            }

            //Default Price, used when others are less or equal to zero
            if (pArticle.Price1UsePromotionPrice && pArticle.Price1Promotion > 0)
            {
                priceDefault = pArticle.Price1Promotion;
            }
            else if (pArticle.Price1 > 0)
            {
                priceDefault = pArticle.Price1;
            }

            if (pArticle != null)
            {
                switch (pPriceType)
                {
                    case PriceType.Price1:
                        priceSource = priceDefault;
                        break;
                    case PriceType.Price2:
                        if (pArticle.Price2UsePromotionPrice && pArticle.Price2Promotion > 0)
                        {
                            priceSource = pArticle.Price2Promotion;
                        }
                        else if (pArticle.Price2 > 0)
                        {
                            priceSource = pArticle.Price2;
                        }
                        if (priceSource <= 0.0m) priceSource = priceDefault;
                        break;
                    case PriceType.Price3:
                        if (pArticle.Price3UsePromotionPrice && pArticle.Price3Promotion > 0)
                        {
                            priceSource = pArticle.Price3Promotion;
                        }
                        else if (pArticle.Price3 > 0)
                        {
                            priceSource = pArticle.Price3;
                        }
                        if (priceSource <= 0.0m) priceSource = priceDefault;
                        break;
                    case PriceType.Price4:
                        if (pArticle.Price4UsePromotionPrice && pArticle.Price4Promotion > 0)
                        {
                            priceSource = pArticle.Price4Promotion;
                        }
                        else if (pArticle.Price4 > 0)
                        {
                            priceSource = pArticle.Price4;
                        }
                        if (priceSource <= 0.0m) priceSource = priceDefault;
                        break;
                    case PriceType.Price5:
                        if (pArticle.Price5UsePromotionPrice && pArticle.Price5Promotion > 0)
                        {
                            priceSource = pArticle.Price5Promotion;
                        }
                        else if (pArticle.Price5 > 0)
                        {
                            priceSource = pArticle.Price5;
                        }
                        if (priceSource <= 0.0m) priceSource = priceDefault;
                        break;
                }
            }

            PricePropertiesSourceMode sourceMode = (!pArticle.PriceWithVat) ? PricePropertiesSourceMode.FromPriceNet : PricePropertiesSourceMode.FromPriceUser;

            //Get Price
            PriceProperties priceProperties = PriceProperties.GetPriceProperties(
              sourceMode,
              pArticle.PriceWithVat,
              priceSource,
              1.0m,
              pArticle.Discount,
              GetDiscountGlobal(),
              priceTax
            );

            //Return PriceProperties Object
            return priceProperties;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //UTF8

        public static byte[] Utf8Encode(String pSource)
        {
            //Encode string to UTF8
            return UTF8Encoding.UTF8.GetBytes(pSource);
        }

        public static String Utf8Decode(byte[] pSource)
        {
            //String from UTF8 encoding
            return UTF8Encoding.UTF8.GetString(pSource);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Network/Hardware

        public static PhysicalAddress GetMacAddress()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                //Skip, Get First Available MacAddress
                //if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet && nic.OperationalStatus == OperationalStatus.Up)
                //{
                return nic.GetPhysicalAddress();
                //}
            }
            return null;
        }

        public static String GetMacAddressString()
        {
            PhysicalAddress physicalAddress = GetMacAddress();
            return physicalAddress.ToString();
        }

        public static string GetFQDN()
        {
            string domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string hostName = Dns.GetHostName();
            string fqdn = String.Empty;

            if (!hostName.Contains(domainName))
                fqdn = hostName + "." + domainName;
            else
                fqdn = hostName;

            return fqdn;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // RSA - Sign

        //Test with
        //string signedR1 = Utils.SHA1SignMessage(@"2010-05-18;2010-05-18T11:22:19;FAC 001/14;3.12;");
        //returns [qE0NX1hZyl26hgP1woTiBM/sWCZZvCumH76IZ2J1MPnbAl......EvX+s4yBQAJhyN8N0aE=]
        //public static string SignDataToSHA1Base64(string pEncryptData, bool pDebug = false)
        //{
        //    return SHA1SignMessage(SettingsApp.RsaPrivateKey, Encoding.UTF8.GetBytes(pEncryptData), pDebug);
        //}

        public static string SignDataToSHA1Base64(string pPrivateKeyPT, string pPrivateKeyAO, string pEncryptData, bool pDebug = false)
        {
            if (ConfigurationManager.AppSettings["cultureFinancialRules"] == "pt-AO")
                return SHA1SignMessage(pPrivateKeyAO, Encoding.UTF8.GetBytes(pEncryptData), pDebug);

            else 
                return SHA1SignMessage(pPrivateKeyPT, Encoding.UTF8.GetBytes(pEncryptData), pDebug);
        }

        private static string SHA1SignMessage(string pPrivateKey, byte[] pMessage, bool pDebug = false)
        {
            String tempPath = Convert.ToString(GlobalFramework.Path["temp"]);

            //Init RSACryptoServiceProvider with Key
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider();

            try
            {
                rsaCryptoServiceProvider.FromXmlString(pPrivateKey);
            }
            catch(Exception ex)
            {
                _log.Debug(ex.Message);
            }
      

            //SHA1 Sign 
            byte[] signature = rsaCryptoServiceProvider.SignData(pMessage, CryptoConfig.MapNameToOID("SHA1"));
            string signatureBase64 = Convert.ToBase64String(signature);

            if (pDebug)
            {
                System.IO.File.WriteAllBytes(tempPath + "encrypt.txt", pMessage);
                System.IO.File.WriteAllBytes(tempPath + "encrypted.sha1", signature);
                System.IO.File.WriteAllText(tempPath + "encrypted.b64", signatureBase64);
            }

            return signatureBase64;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //MD5 Hash File

        public static string MD5HashFile(string file)
        {
            string result = string.Empty;

            try
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 8192);
                md5.ComputeHash(stream);
                stream.Close();

                byte[] hash = md5.Hash;
                StringBuilder sb = new StringBuilder();

                foreach (byte b in hash)
                {
                    sb.Append(string.Format("{0:X2}", b));
                }
                result = sb.ToString().ToLower();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Streams

        // Convert String to System.IO.Stream
        public static MemoryStream StringToStream(string input)
        {
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);

            return stream;
        }

        public static string StreamToString(Stream stream)
        {
            // convert stream to string
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //XPO

        public static string GetValueFromResultSet(SelectedData pSelectedData, int pIndex, string pField)
        {
            string result = "";
            for (int i = 0; i < pSelectedData.ResultSet[0].Rows.Length; i++)
            {
                if (("" + pSelectedData.ResultSet[0].Rows[i].Values[0]).Equals(pField, StringComparison.OrdinalIgnoreCase))
                {
                    result = "" + pSelectedData.ResultSet[1].Rows[pIndex].Values[i];
                    break;
                }
            }
            result = result.Replace("\\s", " ");
            return (result);
        }

        public static XPGuidObject GetXPGuidObjectFromCriteria(Type pXPGuidObjectType, string pCriteriaFilter)
        {
            return GetXPGuidObjectFromCriteria(GlobalFramework.SessionXpo, pXPGuidObjectType, pCriteriaFilter);
        }

        public static XPGuidObject GetXPGuidObjectFromCriteria(Session pSession, Type pXPGuidObjectType, string pCriteriaFilter)
        {
            CriteriaOperator criteria = CriteriaOperator.Parse(pCriteriaFilter);
            XPGuidObject result = (GlobalFramework.SessionXpo.FindObject(pXPGuidObjectType, criteria) as XPGuidObject);
            return result;
        }

        //Helper Method to get XPCollections
        public static XPCollection GetXPCollectionFromCriteria(Type pXPGuidObjectType)
        {
            return GetXPCollectionFromCriteria(GlobalFramework.SessionXpo, pXPGuidObjectType, null, null);
        }

        public static XPCollection GetXPCollectionFromCriteria(Session pSession, Type pXPGuidObjectType)
        {
            return GetXPCollectionFromCriteria(pSession, pXPGuidObjectType, null, null);
        }

        public static XPCollection GetXPCollectionFromCriteria(Session pSession, Type pXPGuidObjectType, CriteriaOperator pCriteriaOperator)
        {
            return GetXPCollectionFromCriteria(pSession, pXPGuidObjectType, pCriteriaOperator, null);
        }

        public static XPCollection GetXPCollectionFromCriteria(Session pSession, Type pXPGuidObjectType, CriteriaOperator pCriteriaOperator, SortingCollection pSortingCollection)
        {
            XPCollection result = new XPCollection(pSession, pXPGuidObjectType, pCriteriaOperator);
            if (pSortingCollection != null) result.Sorting = pSortingCollection;
            return result;
        }

        //Use This Way
        //foreach (SelectStatementResultRow row in xPSelectData.Data) 
        //{
        //  _log.Debug(string.Format("Message: [{0}]", row.Values[xPSelectData.GetFieldIndex("ButtonLabel")].ToString()));
        //}
        public static XPSelectData GetSelectedDataFromQuery(string pSql)
        {
            return GetSelectedDataFromQuery(GlobalFramework.SessionXpo, pSql);
        }

        public static XPSelectData GetSelectedDataFromQuery(Session pSession, string pSql)
        {
            SelectedData xpoSelectedData = pSession.ExecuteQueryWithMetadata(pSql);
            XPSelectData xPSelectData = new XPSelectData(xpoSelectedData);
            return xPSelectData;
        }

        public static DataTable GetDataTableFromQuery(string pSql)
        {
            return GetDataTableFromQuery(GlobalFramework.SessionXpo, pSql);
        }

        public static DataTable GetDataTableFromQuery(Session pSession, string pSql)
        {
            //Get SelectedData
            XPSelectData xPSelectData = GetSelectedDataFromQuery(pSession, pSql);
            //Init DataTable
            DataTable resultDataTable = new DataTable();

            //Add Columns
            String fieldName;
            String fieldType;
            String fieldValue;

            foreach (SelectStatementResultRow row in xPSelectData.Meta)
            {
                fieldName = row.Values[0].ToString();
                fieldType = row.Values[2].ToString();
                resultDataTable.Columns.Add(fieldName, typeof(string));
            }

            //Add Rows
            foreach (SelectStatementResultRow rowData in xPSelectData.Data)
            {
                //Init a new DataRow
                String[] dataRow = new String[xPSelectData.Meta.Length];

                foreach (SelectStatementResultRow rowMeta in xPSelectData.Meta)
                {
                    fieldName = rowMeta.Values[0].ToString();
                    fieldType = rowMeta.Values[2].ToString();
                    //Check if is Not Null
                    if (rowData.Values[xPSelectData.GetFieldIndex(fieldName)] != null)
                    {
                        fieldValue = FormatDataTableFieldFromType(rowData.Values[xPSelectData.GetFieldIndex(fieldName)].ToString(), fieldType);
                    }
                    else
                    {
                        fieldValue = String.Empty;
                    }
                    dataRow[xPSelectData.GetFieldIndex(fieldName)] = fieldValue;
                }
                //resultDataTable.Rows.Add(rowData.Values[xPSelectData.GetFieldIndex(fieldName)].ToString());
                resultDataTable.Rows.Add(dataRow);
            }

            return resultDataTable;
        }

        /// <summary>
        /// Format Fields based on Type . ex Decimals, Doubles, Int64
        /// </summary>
        /// <param name="pFieldValue"></param>
        /// <param name="pFieldType"></param>
        /// <returns></returns>
        public static String FormatDataTableFieldFromType(string pFieldValue, string pFieldType)
        {
            String resultFieldValue = String.Empty;

            switch (pFieldType)
            {
                case "Decimal":
                //Required for SQLite, it uses Int64 has Decimal?
                case "Int64":
                    Decimal valueDecimal;
                    Decimal.TryParse(pFieldValue, out valueDecimal);
                    resultFieldValue = DecimalToString(valueDecimal);
                    break;
                case "Double":
                    Double valueDouble;
                    Double.TryParse(pFieldValue, out valueDouble);
                    resultFieldValue = DoubleToString(valueDouble);
                    break;
                default:
                    resultFieldValue = pFieldValue;
                    break;
            }
            return resultFieldValue;
        }

        public static SortingCollection GetXPCollectionDefaultSortingCollection()
        {
            SortingCollection sortingCollection = new SortingCollection();
            sortingCollection.Add(new SortProperty("Ord", DevExpress.Xpo.DB.SortingDirection.Ascending));
            //sortingCollection.Add(new SortProperty("Designation", DevExpress.Xpo.DB.SortingDirection.Ascending));
            return sortingCollection;
        }

        public static int GetNextTableFieldInt(string pTable, string pField)
        {
            return GetNextTableFieldInt(pTable, pField, String.Empty);
        }

        public static int GetNextTableFieldInt(string pTable, string pField, string pFilter)
        {
            int result = -1;
            string filter = (pFilter != String.Empty) ? string.Format(" WHERE {0}", pFilter) : String.Empty;
            try
            {
                string sql = string.Format("SELECT MAX({0}) FROM {1}{2};", pField, pTable, filter);
                var resultInt = GlobalFramework.SessionXpo.ExecuteScalar(sql);
                if (resultInt != null)
                {
                    _log.Debug(string.Format("GetNextTableFieldInt(): resultInt.GetType(): [{0}]", resultInt.GetType()));

                    if (resultInt.GetType() == typeof(Int16))
                    {
                        result = (Int16)((Int16)resultInt + 1);
                    }
                    else if (resultInt.GetType() == typeof(Int32))
                    {
                        result = (Int32)((Int32)resultInt + 1);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return result;
            }

            if (result == -1) result = 1;
            return result;
        }

        /// <summary>
        /// Used to get Guids from Queries. Essencial because MSSqlServer (Returns Guid) and Other Dbs like Mysql (Returns String)
        /// </summary>
        /// <param name="Object"></param>
        /// <returns></returns>
        public static Guid GetGuidFromQuery(string pSql)
        {
            return GetGuidFromQuery(GlobalFramework.SessionXpo, pSql);
        }

        public static Guid GetGuidFromQuery(Session pSession, string pSql)
        {
            var resultField = pSession.ExecuteScalar(pSql);

            if (resultField != null)
            {
                if (resultField.GetType() == typeof(string))
                {
                    return new Guid((string)resultField);
                }
                else if (resultField.GetType() == typeof(Guid))
                {
                    return (Guid)resultField;
                }
            }

            return Guid.Empty;
        }

        public static XPGuidObject GetXPGuidObjectFromField(Type pType, string pSearchField, string pSearchValue)
        {
            return GetXPGuidObjectFromField(GlobalFramework.SessionXpo, pType, pSearchField, pSearchValue);
        }

        public static XPGuidObject GetXPGuidObjectFromField(Session pSession, Type pType, string pSearchField, string pSearchValue)
        {
            string executeSql = string.Format(@"SELECT Oid FROM {0} WHERE (Disabled IS NULL OR Disabled  <> 1) AND {1} = '{2}';", pType.Name, pSearchField, pSearchValue);
            Guid guid = GetGuidFromQuery(pSession, executeSql);
            if (guid != Guid.Empty)
            {
                return (XPGuidObject)GetXPGuidObject(pSession, pType, guid);
            }
            else
            {
                return null;
            }
        }

        public static dynamic GetObjectFromField(Session pSession, Type pType, string pSearchField, string pSearchValue)
        {
            string executeSql = string.Format(@"SELECT Oid FROM {0} WHERE (Disabled IS NULL OR Disabled  <> 1) AND {1} = '{2}';", pType.Name, pSearchField, pSearchValue);
            Guid guid = GetGuidFromQuery(pSession, executeSql);
            if (guid != Guid.Empty)
            {
                XPClassInfo classInfo = pSession.GetClassInfo(pType);
                return (pSession.GetObjectByKey(classInfo, guid));
            }
            else
            {
                return null;
            }
        }

        public static dynamic GetObjectFromSession(Session pSession, Type pType, Guid pOid)
        {
            XPClassInfo classInfo = pSession.GetClassInfo(pType);
            return (pSession.GetObjectByKey(classInfo, pOid));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Type Helpers

        public static bool IsNullable(Type pType)
        {
            return (pType.IsGenericType && pType.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Localization

        //public static decimal InvariantCultureStringToDecimal(string pInput)
        //{
        //  //Always replace 
        //  pInput = pInput.Replace(',', '.');
        //  //always expect dot character to delimit your decimal digits regardless of what is set in current thread's culture
        //  return decimal.Parse(pInput, CultureInfo.InvariantCulture);
        //}

        public static decimal StringToDecimal(string pInput)
        {
            decimal result;

            NumberStyles numberStyle = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent;

            if (!decimal.TryParse(pInput, numberStyle, GlobalFramework.CurrentCulture.NumberFormat, out result))
            {
                decimal.TryParse(pInput, numberStyle, CultureInfo.InvariantCulture.NumberFormat, out result);
            }

            return result;
        }

        public static string DecimalToString(decimal pInput)
        {
            return DecimalToString(pInput, GlobalFramework.CurrentCulture);
        }

        public static string DecimalToString(decimal pInput, CultureInfo pCulture)
        {
            return DecimalToString(pInput, pCulture, SettingsApp.DecimalFormat);
        }

        public static string DecimalToString(decimal pInput, string pDecimalFormat)
        {
            return DecimalToString(pInput, GlobalFramework.CurrentCulture, pDecimalFormat);
        }

        public static string DecimalToString(decimal pInput, CultureInfo pCulture, string pDecimalFormat)
        {
            return pInput.ToString(pDecimalFormat, pCulture.NumberFormat);
        }

        public static string DecimalToStringCurrency(decimal pInput)
        {
            return DecimalToStringCurrency(pInput, SettingsApp.ConfigurationSystemCurrency.Acronym);
        }

        public static string DecimalToStringCurrency(decimal pInput, string pCurrencyAcronym)
        {
            return string.Format("{0}{1}", DecimalToString(pInput), pCurrencyAcronym);
        }

        public static string DoubleToString(double pInput)
        {
            return pInput.ToString(SettingsApp.DecimalFormat, GlobalFramework.CurrentCulture.NumberFormat);
        }

        public static string DoubleToStringCurrency(double pInput)
        {
            return string.Format("{0}{1}", DoubleToString(pInput), SettingsApp.ConfigurationSystemCurrency.Acronym);
        }

        //Used to Format a String into a Decimal Format String ex (string) 1 converted to (string) 1.00
        public static string StringToDecimalAndToStringAgain(string pInput)
        {
            string result = DecimalToString(StringToDecimal(pInput));

            //Linux Protection Crash, if value is ,00 its Crash, add 0 to prevent it, ex 0,00
            if (result[0] == Convert.ToChar(GlobalFramework.CurrentCulture.NumberFormat.NumberDecimalSeparator))
            {
                result = string.Format("{0}{1}", 0, result);
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Audit - Currently Working in Progress

        public static bool Audit(string pAuditTypeToken, string pDescription = "")
        {
            return Audit(
                GlobalFramework.SessionXpo,
                (GlobalFramework.LoggedUser != null) ? GlobalFramework.LoggedUser : null,
                (GlobalFramework.LoggedTerminal != null) ? GlobalFramework.LoggedTerminal : null,
                pAuditTypeToken,
                pDescription
            );
        }

        public static bool Audit(Session pSession, sys_userdetail pLoggedUser, pos_configurationplaceterminal pLoggedTerminal, string pAuditTypeToken)
        {
            return Audit(pSession, pLoggedUser, pLoggedTerminal, pAuditTypeToken, String.Empty);
        }

        public static bool Audit(Session pSession, sys_userdetail pLoggedUser, pos_configurationplaceterminal pLoggedTerminal, string pAuditTypeToken, string pDescription = "")
        {
            bool result = false;
            DateTime dateTime = CurrentDateTimeAtomic();
            string executeSql = string.Format(@"SELECT Oid FROM sys_systemaudittype WHERE (Disabled IS NULL or Disabled  <> 1) AND Token = '{0}';", pAuditTypeToken);

            //Check if has a Valid LoggedUser else Assign NULL to INSERT, usefull to log stuff when User is not Yet Logged
            //string loggedUserOid = (pLoggedUser != null) ? string.Format("'{0}'", pLoggedUser.Oid.ToString()) : "NULL";

            try
            {
                //Get auditType Guid from Query
                Guid guidAuditType = GetGuidFromQuery(executeSql);

                if (!guidAuditType.Equals(Guid.Empty))
                {
                    //Fresh User and Terminal, to prevent Object Delection Problem
                    sys_userdetail xpoUserDetail = (pLoggedUser != null) ? (sys_userdetail)GetXPGuidObject(typeof(sys_userdetail), pLoggedUser.Oid) : null;
                    pos_configurationplaceterminal xpoTerminal = (pos_configurationplaceterminal)GetXPGuidObject(typeof(pos_configurationplaceterminal), pLoggedTerminal.Oid);
                    //get AuditType Object
                    sys_systemaudittype xpoAuditType = (sys_systemaudittype)GetXPGuidObject(typeof(sys_systemaudittype), guidAuditType);
                    string description = (pDescription != String.Empty) ? pDescription
                      : (xpoAuditType.ResourceString != null && resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], xpoAuditType.ResourceString) != null)
                      ? resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], xpoAuditType.ResourceString) : xpoAuditType.Designation;

                    sys_systemaudit systemAudit = new sys_systemaudit(pSession)
                    {
                        Date = dateTime,
                        Description = description,
                        UserDetail = xpoUserDetail,
                        Terminal = xpoTerminal,
                        AuditType = xpoAuditType
                    };
                    systemAudit.Save();

                    _log.Debug(string.Format("Audit(): {0} > {1}", pAuditTypeToken, description));

                    result = true;
                }
                else
                {
                    string exceptionMessage = string.Format("Invalid AuditType: [{0}]", pAuditTypeToken);
                    _log.Error(exceptionMessage);
                    //TriggerError
                    throw (new Exception(exceptionMessage));
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Audit(): {0} > {1}", pAuditTypeToken, ex.Message), ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //System

        public static bool CreateDirectory(string pPath)
        {
            bool result = false;

            try
            {
                if (Directory.Exists(pPath))
                {
                    result = true;
                }
                else if (!Directory.Exists(pPath))
                {
                    DirectoryInfo di = Directory.CreateDirectory(pPath);
                    result = true;
                }
            }
            catch (IOException ex)
            {
                _log.Error(ex.Message, ex);
            }
            return result;
        }

        public static void ExecuteUnixCommand(string pFileName, string pArguments = "")
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = pFileName;
            if (pArguments != String.Empty) proc.StartInfo.Arguments = pArguments;
            proc.Start();
            proc.WaitForExit();
        }

        /* ERR201810#15 - Database backup issues */
        //public static String RelativePath(string pFileOrPath)
        //{
        //    string result = pFileOrPath;
        //    try
        //    {
        //        if (pFileOrPath.StartsWith(@"\\"))
        //        {
        //        }
        //        else
        //        {
        //            Uri uri1 = new Uri(pFileOrPath);
        //            Uri uri2 = new Uri(Environment.CurrentDirectory + "/");
        //            result = uri2.MakeRelativeUri(uri1).ToString();
        //            //Require to Unescape string escaped by MakeRelativeUri, ex %20 etc, to prevent error when user uses a filename with space(%20) for ex, and other escaped characters
        //            result = Uri.UnescapeDataString(result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Error(ex.Message, ex);
        //    }
        //    return (result);
        //}

        public static string ProductVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                return string.Format("v{0}", fileVersionInfo.ProductVersion);
            }
        }

        //Mono CLR version + Windows CLR version
        public static string ProductVersionCLR
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().ImageRuntimeVersion;
            }
        }

        //Get Product Assembly
        public static Assembly ProductAssembly
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly();
            }
        }

        //TODO: Changed to ExecuteExternalProcess by Mario, to be Generic, Was ShowPOS_Composer
        public static void ExecuteExternalProcess(string pExe)
        {
            ExecuteExternalProcess(pExe, String.Empty);
        }
        public static void ExecuteExternalProcess(string pExe, string pArgs)
        {
            try
            {
                var proc = new Process();

                proc.StartInfo.FileName = pExe;
                if (pArgs != null || pArgs != String.Empty) proc.StartInfo.Arguments = pArgs;
                proc.Start();

                // GlobalApp.WindowReportsWinForm = new logicpos.Classes.Gui.WinForms.BackOffice.Windows.BackOfficeReportWindow();
                _log.Debug("Iniciar " + pExe);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public static void ClearCache()
        {
            try
            {
                string tmpPath = OSSlash(Convert.ToString(GlobalFramework.Path["cache"]));
                if (Directory.Exists(tmpPath))
                {
                    Directory.Delete(tmpPath, true);
                }
                CreateDirectory(tmpPath);
            }
            catch (IOException ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Cross Plataform

        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        //Return os windows,unix,macosx
        //http://docs.go-mono.com/index.aspx?link=T%3ASystem.PlatformID
        public static String OSVersion()
        {
            String result = String.Empty;
            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;

            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    result = "windows";
                    break;
                case PlatformID.Unix:
                    result = "unix";
                    break;
                case PlatformID.MacOSX:
                    result = "macosx";
                    break;
                default:
                    result = String.Empty;
                    break;
            }
            return result;
        }

        public static bool UsePosPDFViewer()
        {
            bool result = false;

            try
            {
                result = Convert.ToBoolean(GlobalFramework.PreferenceParameters["USE_POS_PDF_VIEWER"]);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return (result);
        }

         public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        //Replace Slash with OSSlash ex windows=\ and Linux=/
        public static String OSSlash(String pPath)
        {
            if (pPath != null)
            {
                //TODO: FIX
                string result = pPath.Replace('\\', '/'/*Path.DirectorySeparatorChar*/);
                //_log.Debug(string.Format("OSSlash(): [{0}] : original pPath:[{1}] : result pPath: [{2}]", Path.DirectorySeparatorChar, pPath, result));
                return result;
            }
            else
            {
                return String.Empty;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Colors

        //Converts a Color to String using TypeConverter, used to Store values in Appsettings
        public static String ColorToString(Color pColor)
        {
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));
                return converter.ConvertToInvariantString(pColor);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return "255, 0, 0";
            }
        }

        //Converts a String to Color using TypeConverter, used to Store values in Appsettings
        public static Color StringToColor(String pColor)
        {
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));
                return (Color)converter.ConvertFromInvariantString(pColor);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return (Color)Color.FromArgb(255, 0, 0);
            }
        }


        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Financial 

        public static string TranslateNumber(string pv_strNumber)
        {
            string[,] lv_strNumExt = new string[10, 30];
            lv_strNumExt[0, 0] = "zero";
            lv_strNumExt[0, 1] = "um";
            lv_strNumExt[0, 2] = "dois";
            lv_strNumExt[0, 3] = "três";
            lv_strNumExt[0, 4] = "quatro";
            lv_strNumExt[0, 5] = "cinco";
            lv_strNumExt[0, 6] = "seis";
            lv_strNumExt[0, 7] = "sete";
            lv_strNumExt[0, 8] = "oito";
            lv_strNumExt[0, 9] = "nove";
            lv_strNumExt[0, 10] = "dez";
            lv_strNumExt[0, 11] = "onze";
            lv_strNumExt[0, 12] = "doze";
            lv_strNumExt[0, 13] = "treze";
            lv_strNumExt[0, 14] = "quatorze";
            lv_strNumExt[0, 15] = "quinze";
            lv_strNumExt[0, 16] = "dezaseis";
            lv_strNumExt[0, 17] = "dezasete";
            lv_strNumExt[0, 18] = "dezoito";
            lv_strNumExt[0, 19] = "dezanove";
            lv_strNumExt[1, 2] = "vinte";
            lv_strNumExt[1, 3] = "trinta";
            lv_strNumExt[1, 4] = "quarenta";
            lv_strNumExt[1, 5] = "cinquenta";
            lv_strNumExt[1, 6] = "sessenta";
            lv_strNumExt[1, 7] = "setenta";
            lv_strNumExt[1, 8] = "oitenta";
            lv_strNumExt[1, 9] = "noventa";
            lv_strNumExt[2, 1] = "cento";
            lv_strNumExt[2, 2] = "duzentos";
            lv_strNumExt[2, 3] = "trezentos";
            lv_strNumExt[2, 4] = "quatrocentos";
            lv_strNumExt[2, 5] = "quinhentos";
            lv_strNumExt[2, 6] = "seicentos";
            lv_strNumExt[2, 7] = "setecentos";
            lv_strNumExt[2, 8] = "oitocentos";
            lv_strNumExt[2, 9] = "novecentos";
            lv_strNumExt[3, 1] = "mil";
            for (int lv_intI = 2; lv_intI < 10; lv_intI++)
            {
                lv_strNumExt[3, lv_intI] = lv_strNumExt[0, lv_intI] + " mil";
            }

            lv_strNumExt[4, 1] = "mil";
            for (int lv_intI = 1; lv_intI < 10; lv_intI++)
            {
                lv_strNumExt[4, lv_intI] = lv_strNumExt[1, lv_intI] + " mil";
            }

            string lv_strResult = "";
            string lv_strNumber = pv_strNumber.Trim().Replace(",", ".");
            string lv_strTmp = "";
            for (int lv_intI = lv_strNumber.Length - 1; lv_intI >= 0; lv_intI--)
            {
                lv_strTmp += lv_strNumber.Substring(lv_intI, 1);
            }

            lv_strNumber = lv_strTmp;
            for (int lv_intI = lv_strNumber.Length - 1; lv_intI >= 0; lv_intI--)
            {
                switch (lv_intI)
                {

                    case 0:
                        int lv_intNum = int.Parse(lv_strNumber.Substring(lv_intI, 1));
                        lv_strResult += lv_strNumExt[lv_intI, lv_intNum] + " e ";
                        break;

                    case 1:
                        lv_intNum = int.Parse(lv_strNumber.Substring(lv_intI, 1));
                        if (lv_intNum > 1)
                        {
                            lv_strResult += lv_strNumExt[lv_intI, lv_intNum] + " e ";
                            if (int.Parse(lv_strNumber.Substring(lv_intI - 1, 1)) == 0)
                            {
                                lv_intI = 0;
                            }
                        }
                        else if (lv_intNum == 1)
                        {
                            lv_intNum = int.Parse(lv_strNumber.Substring(0, 1));
                            lv_strResult += lv_strNumExt[lv_intI - 1, 10 + lv_intNum] + " e ";
                            lv_intI = 0;
                        }
                        break;

                    case 2:
                        lv_intNum = int.Parse(lv_strNumber.Substring(lv_intI, 1));
                        if (lv_intNum >= 1)
                        {
                            lv_strResult += lv_strNumExt[lv_intI, lv_intNum] + " e ";
                        }
                        break;

                    case 3:
                        lv_intNum = int.Parse(lv_strNumber.Substring(lv_intI, 1));
                        if (lv_intNum >= 1)
                        {
                            lv_strResult += lv_strNumExt[lv_intI, lv_intNum] + " e ";
                        }

                        break;
                }
            }

            if (lv_strResult.EndsWith(" e "))
            {
                lv_strResult = lv_strResult.Substring(0, lv_strResult.Length - 3) + " ";
            }
            if (lv_strResult.Equals("cento "))
            {
                lv_strResult = "cem ";
            }

            return (lv_strResult);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //PartialPayments

        public static decimal GetPartialPaymentPayedItems(Guid pDocumentOrderMain, Guid pArticle)
        {
            return GetPartialPaymentPayedItems(GlobalFramework.SessionXpo, pDocumentOrderMain, pArticle);
        }

        public static decimal GetPartialPaymentPayedItems(Session pSession, Guid pDocumentOrderMain, Guid pArticle)
        {
            decimal result = 0.0m;

            string sql = string.Format(@"
                SELECT 
                  SUM(fdQuantity) AS Quantity 
                FROM 
                  view_documentfinance 
                WHERE 
                  fmSourceOrderMain = '{0}' AND 
                  fdArticle = '{1}'
                ;"
              , pDocumentOrderMain
              , pArticle
            );
            //_log.Debug(string.Format("sql: [{0}]", sql));
            var partialPayedItems = pSession.ExecuteScalar(sql);

            return (partialPayedItems != null && Convert.ToDecimal(partialPayedItems) > 0)
              ? Convert.ToDecimal(partialPayedItems)
              : result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Preference Parameters

        public static Dictionary<string, string> GetPreferencesParameters()
        {
            return GetPreferencesParameters(GlobalFramework.SessionXpo);
        }

        public static Dictionary<string, string> GetPreferencesParameters(Session pSession)
        {
            try
            {
                Dictionary<string, string> resultPreferences = new Dictionary<string, string>();

                SortingCollection sortingCollection = new SortingCollection();
                sortingCollection.Add(new SortProperty("Ord", SortingDirection.Ascending));
                CriteriaOperator criteriaOperator = CriteriaOperator.Parse("Disabled = 0 OR Disabled is NULL");
                XPCollection xpcPreferenceParameter = new XPCollection(pSession, typeof(cfg_configurationpreferenceparameter), criteriaOperator);

                foreach (cfg_configurationpreferenceparameter item in xpcPreferenceParameter)
                {
                    resultPreferences.Add(item.Token, item.Value);
                }
                return resultPreferences;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// Get value from dictionary with LINQ
        /// Deprecated: better to get it with key ex GlobalFramework.PreferenceParameters[token]
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        //public static string GetPreferencesValue(string token)
        //{
        //    string result = string.Empty;

        //    try
        //    {
        //        result = GlobalFramework.PreferenceParameters.FirstOrDefault(k => k.Key.Contains(token)).Value;
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Debug(ex.Message, ex);
        //    }

        //    return result;
        //}

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //File

        public static void WriteTextToFile(string pFileName, string pText)
        {
            WriteTextToFile(pFileName, pText, FileMode.Append);
        }

        public static void WriteTextToFile(string pFileName, string pText, FileMode pFileMode)
        {
            using (FileStream fs = new FileStream(pFileName, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(pText);
                }
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //CSV and Comma Delimited Strings

        //Convert a CommaDelimited String into List of Strings
        public static List<string> CommaDelimitedStringToStringList(string pInput)
        {
            return CommaDelimitedStringToStringList(pInput, ';');
        }

        public static List<string> CommaDelimitedStringToStringList(string pInput, char pSeparator)
        {
            string[] arrayString = pInput.Split(pSeparator);
            List<string> listResult = new List<string>();

            for (int i = 0; i < arrayString.Length; i++)
            {
                listResult.Add(arrayString[i]);
            }
            return listResult;
        }

        public static List<int> CommaDelimitedStringToIntList(string pInput)
        {
            return CommaDelimitedStringToIntList(pInput, ',');
        }

        public static List<int> CommaDelimitedStringToIntList(string pInput, char pSeparator)
        {
            string[] arrayString = pInput.Split(pSeparator);
            List<int> listResult = new List<int>();

            for (int i = 0; i < arrayString.Length; i++)
            {
                int integer;
                bool res = int.TryParse(arrayString[i], out integer);

                if (res)
                {
                    listResult.Add(integer);
                }

            }
            return listResult;
        }

        //Convert a CommaDelimited Generic List into a Comma Delimited String
        public static string StringListToCommaDelimitedString<T>(List<T> pList)
        {
            return StringListToCommaDelimitedString(pList, ';');
        }

        public static string StringListToCommaDelimitedString<T>(List<T> pList, char pSeparator)
        {
            string result = String.Empty;

            try
            {
                int i = 0;
                foreach (var item in pList)
                {
                    i++;
                    result += item.ToString();
                    if (i < pList.Count) result += pSeparator.ToString();
                }

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        public static Dictionary<string, string> CSVFileToDictionary(string pFilePath)
        {
            return CSVFileToDictionary(pFilePath, ',');
        }

        //Convert a CSV File to Dictionary of Strings
        public static Dictionary<string, string> CSVFileToDictionary(string pFilePath, char pSplitter)
        {
            bool debug = false;
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (File.Exists(pFilePath))
            {
                StreamReader streamReader = new StreamReader(File.OpenRead(pFilePath));

                try
                {
                    while (!streamReader.EndOfStream)
                    {
                        var line = streamReader.ReadLine();
                        var values = line.Split(pSplitter);

                        if (values.Length == 2 && !String.IsNullOrEmpty(values[0]) && !String.IsNullOrEmpty(values[1]))
                        {
                            string val1 = values[0];
                            string val2 = values[1];

                            if (!result.ContainsKey(val1))
                            {
                                if (debug) _log.Debug(string.Format("CSVFileToDictionary Add: [{0}],[{1}]", val1, val2));
                                result.Add(val1, val2);
                            }
                        }
                    }
                    streamReader.Close();
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                }
                finally
                {
                    streamReader.Close();
                }
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Lists

        //Merge List<List<T>> with LINQ Concat and ToList, return a Distinct List of <T>
        //Test With
        //List<string> list1 = new List<string>(); list1.Add("def"); list1.Add("abc"); list1.Add("ghi");
        //List<string> list2 = new List<string>(); list2.Add("jkl"); list2.Add("abc"); list2.Add("mno");
        //List<string> list3 = new List<string>(); list3.Add("pqr"); list3.Add("stu"); list3.Add("jkl");
        //List<List<string>> list4 = new List<List<string>>(); list4.Add(list1); list4.Add(list2); list4.Add(list3);
        //List<string> list5 = MergeGenericLists(list4);
        //list5.Sort();
        public static List<T> MergeGenericLists<T>(List<List<T>> pLists)
        {
            List<T> result = new List<T>();

            for (int i = 0; i < pLists.Count; i++)
            {
                result = result.Concat(pLists[i]).ToList();
            }

            return result.Distinct().ToList();
        }

        public static List<string> FilterList(List<string> pMessageList, string pSearch)
        {
            return FilterList(pMessageList, new List<string> { pSearch });
        }

        public static List<string> FilterList(List<string> pMessageList, List<string> pSearchList)
        {
            bool debug = false;
            List<string> result = new List<string>();

            try
            {
                var resultFromLinq =
                    from name in pMessageList
                    from startWithWord in pSearchList
                    where name.StartsWith(startWithWord)
                    select name
                ;

                foreach (string str in resultFromLinq)
                {
                    result.Add(str);
                    if (debug) _log.Debug(string.Format("Message: [{0}]", str));
                }

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Permissions

        public static Dictionary<string, bool> GetUserPermissions()
        {
            return GetUserPermissions(GlobalFramework.LoggedUser);
        }

        public static Dictionary<string, bool> GetUserPermissions(sys_userdetail pUser)
        {
            bool debug = false;
            Dictionary<string, bool> resultPermissions = new Dictionary<string, bool>();

            try
            {
                //Shared Permissions For All Users
                resultPermissions.Add("BACKOFFICE_MAN_SYSTEM_MENU_MENU", true);
                resultPermissions.Add("BACKOFFICE_MAN_SYSTEM_POS_MENU", true);
                resultPermissions.Add("BACKOFFICE_MAN_SYSTEM_QUIT_MENU", true);
                resultPermissions.Add("BACKOFFICE_MAN_SYSTEM_NOTIFICATION_MENU", true);

                if (pUser != null)
                {
                    if (pUser.Profile != null)
                    {
                        foreach (sys_userpermissionprofile item in pUser.Profile.Permissions)
                        {
                            if (debug) _log.Debug(string.Format("Permission: Token [{0}], Granted [{1}]", item.PermissionItem.Token, item.Granted));
                            resultPermissions.Add(item.PermissionItem.Token, item.Granted);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //If error Detect Check Duplicated PermissionItems in Profile
                //SELECT upiToken,COUNT(*) as Count FROM view_userprofile WHERE uprOid = '1626e21f-75e6-429e-b0ac-edb755e733c2' GROUP BY upiToken ORDER BY COUNT(*) DESC,upiCode;
                _log.Error(ex.Message, ex);
            }
            return resultPermissions;
        }

        public static bool HasPermissionTo(String pToken)
        {
            bool result = false;

            if (GlobalFramework.LoggedUserPermissions != null && GlobalFramework.LoggedUserPermissions.ContainsKey(pToken))
            {
                result = GlobalFramework.LoggedUserPermissions[pToken];
            }
            else
            {
                result = false;
            }

            //_log.Debug(string.Format("HasPermissionTo({0}): {1}", pToken, result));
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Notifications

        //Create SystemNotification
        public static void SystemNotification()
        {
            SystemNotification(GlobalFramework.SessionXpo);
        }

        public static void SystemNotification(Session pSession)
        {
            _log.Debug("void FrameworkUtils.SystemNotification(Session pSession) :: pSession: " + pSession.ToString());
            bool debug = true;

            //Filter 
            //To All: "NotificationType = '{0}'"
            //To User: "NotificationType = '{0}' AND UserLastRead = '{1}'"
            //To Terminal: "NotificationType = '{0}' AND TerminalLastRead = '{1}'"
            //To User Terminal: "NotificationType = '{0}' AND UserLastRead = '{1}' AND TerminalLastRead = '{1}'"
            try
            {
                //Settings
                string cultureFinancialRules = GlobalFramework.Settings["cultureFinancialRules"];

                //Local Vars
                uint ord = 1;
                sys_systemnotificationtype systemNotificationType;
                sys_systemnotification systemNotification;
                CriteriaOperator criteriaOperator;
                XPCollection xpcSystemNotification;

                //:::: Notification : Edit Configuration Parameters ::::
                //Check existing Notification before Create
                //systemNotificationType = (sys_systemnotificationtype)pSession.GetObjectByKey(typeof(sys_systemnotificationtype), SettingsApp.XpoOidSystemNotificationTypeEditConfigurationParameters);
                //if (systemNotificationType != null)
                //{
                //    criteriaOperator = CriteriaOperator.Parse(string.Format("NotificationType = '{0}'", SettingsApp.XpoOidSystemNotificationTypeEditConfigurationParameters));
                //    xpcSystemNotification = new XPCollection(pSession, typeof(sys_systemnotification), criteriaOperator);
                //    //Create Notification
                //    if (xpcSystemNotification.Count == 0)
                //    {
                //        systemNotification = new sys_systemnotification(pSession);
                //        systemNotification.Ord = ord;
                //        systemNotification.NotificationType = systemNotificationType;
                //        systemNotification.Message = systemNotificationType.Message;
                //        systemNotification.Save();
                //        ord++;
                //        if (debug) _log.Debug(string.Format("Notification created: [{0}]", systemNotificationType.Designation));
                //    }
                //};

                //:::: Notification : WelcomeMessage ::::
                //Check existing Notification before Create
                systemNotificationType = (sys_systemnotificationtype)pSession.GetObjectByKey(typeof(sys_systemnotificationtype), SettingsApp.XpoOidSystemNotificationTypeNewTerminalRegistered);
                if (systemNotificationType != null)
                {
                    criteriaOperator = CriteriaOperator.Parse(string.Format("NotificationType = '{0}' AND TerminalLastRead = '{1}'", SettingsApp.XpoOidSystemNotificationTypeNewTerminalRegistered, GlobalFramework.LoggedTerminal.Oid));
                    xpcSystemNotification = new XPCollection(pSession, typeof(sys_systemnotification), criteriaOperator);
                    //Create Notification
                    if (xpcSystemNotification.Count == 0)
                    {
                        systemNotification = new sys_systemnotification(pSession);
                        systemNotification.Ord = ord;
                        systemNotification.NotificationType = systemNotificationType;
                        systemNotification.Message = string.Format(systemNotificationType.Message, GlobalFramework.LoggedTerminal.Designation);
                        systemNotification.Save();
                        ord++;
                        if (debug) _log.Debug(string.Format("Notification created: [{0}]", systemNotificationType.Designation));
                    }
                };

                //:::: Notification : RequestPasswordChange ::::
                //Check existing Notification before Create
                /* DISABLE Currently in Progress
                systemNotificationType = (SystemNotificationType)pSession.GetObjectByKey(typeof(SystemNotificationType), SettingsApp.XpoOidSystemNotificationTypeFirstLoginRequestPasswordChange);
                if (systemNotificationType != null)
                {
                    criteriaOperator = CriteriaOperator.Parse(string.Format("NotificationType = '{0}' AND UserLastRead = '{1}'", SettingsApp.XpoOidSystemNotificationTypeFirstLoginRequestPasswordChange, GlobalFramework.LoggedUser));
                    xpcSystemNotification = new XPCollection(pSession, typeof(SystemNotification), criteriaOperator);
                    //Create Notification
                    if (xpcSystemNotification.Count == 0)
                    {
                        systemNotification = new SystemNotification(pSession);
                        systemNotification.Ord = ord;
                        systemNotification.NotificationType = systemNotificationType;
                        systemNotification.Message = string.Format(systemNotificationType.Message, GlobalFramework.LoggedUser.Login, GlobalFramework.LoggedUser.Name);
                        systemNotification.Save();
                        ord++;
                        if (debug) _log.Debug(string.Format("Notification created: [{0}]", systemNotificationType.Designation));
                    }
                };
                */

                //:::: Notification : ProprietaryTestMessage ::::
                //Check existing Notification before Create
                /*TEMP REMOVED FOR MediaNova
                systemNotificationType = (SystemNotificationType)pSession.GetObjectByKey(typeof(SystemNotificationType), SettingsApp.XpoOidSystemNotificationTypeProprietaryTestMessage);
                criteriaOperator = CriteriaOperator.Parse(string.Format("NotificationType = '{0}' AND UserTarget = '{1}'", SettingsApp.XpoOidSystemNotificationTypeProprietaryTestMessage, userProprietary));
                xpcSystemNotification = new XPCollection(pSession, typeof(SystemNotification), criteriaOperator);
                //Create Notification
                if (xpcSystemNotification.Count == 0)
                {
                    UserDetail user = (UserDetail)pSession.GetObjectByKey(typeof(UserDetail), new Guid(userProprietary));
                    systemNotification = new SystemNotification(pSession)
                    {
                        Ord = ord,
                        NotificationType = systemNotificationType,
                        Message = string.Format(systemNotificationType.Message, user.Name),
                        UserTarget = user
                    };
                    systemNotification.Save();
                    ord++;
                    if (debug) _log.Debug(string.Format("Notification created: [{0}]", systemNotificationType.Designation));
                }
                */

                //Financial Culture Notifications
                //switch (GlobalFramework.CurrentCulture.ToString())
                switch (cultureFinancialRules)
                {
                    case "pt-PT":
                        int defaultBackDaysForInvoice = SettingsApp.XpoOidSystemNotificationDaysBackWhenFiltering;

                        //:::: Notification : CurrentAccountDocumentsToInvoice ::::
                        //ProcessFinanceDocumentToInvoice to Create Notification in Spool for CurrentAccount Documents
                        //systemNotificationType = (SystemNotificationType)pSession.GetObjectByKey(typeof(SystemNotificationType), SettingsApp.XpoOidSystemNotificationTypeCurrentAccountDocumentsToInvoice);
                        systemNotification = ProcessFinanceDocumentToInvoice(pSession, SettingsApp.XpoOidSystemNotificationTypeCurrentAccountDocumentsToInvoice, SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput, "(Payed = 0 OR Payed IS NULL)", defaultBackDaysForInvoice);
                        if (systemNotification != null)
                        {
                            systemNotification.Ord = ord; systemNotification.Save(); ord++;
                            if (debug) _log.Debug(string.Format("Notification created: [{0}]", systemNotificationType.Designation));
                        };

                        //:::: Notification : ConsignationInvoiceDocumentsToInvoice ::::
                        //ProcessFinanceDocumentToInvoice to Create Notification in Spool for CurrentAccount Documents
                        //systemNotificationType = (SystemNotificationType)pSession.GetObjectByKey(typeof(SystemNotificationType), SettingsApp.XpoOidSystemNotificationTypeConsignationInvoiceDocumentsToInvoice);
                        systemNotification = ProcessFinanceDocumentToInvoice(pSession, SettingsApp.XpoOidSystemNotificationTypeConsignationInvoiceDocumentsToInvoice, SettingsApp.XpoOidDocumentFinanceTypeConsignationInvoice, "(DocumentChild IS NULL)", defaultBackDaysForInvoice);
                        if (systemNotification != null)
                        {
                            systemNotification.Ord = ord; systemNotification.Save(); ord++;
                            if (debug) _log.Debug(string.Format("Notification created: [{0}]", systemNotificationType.Designation));
                        };

                        //:::: Notification : SaftDocumentType.MovementOfGoodsToInvoice ::::
                        //ProcessFinanceDocumentToInvoice to Create Notification in Spool for CurrentAccount Documents
                        systemNotification = ProcessFinanceDocumentToInvoice(pSession, SettingsApp.XpoOidSystemNotificationTypeSaftDocumentTypeMovementOfGoods, SaftDocumentType.MovementOfGoods, "(DocumentChild IS NULL AND DocumentStatusStatus = 'N')", defaultBackDaysForInvoice);
                        if (systemNotification != null)
                        {
                            systemNotification.Ord = ord; systemNotification.Save(); ord++;
                            if (debug) _log.Debug(string.Format("Notification created: [{0}]", systemNotificationType.Designation));
                        };

                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //Overload From DocumentType Guid
        public static sys_systemnotification ProcessFinanceDocumentToInvoice(Session pSession, Guid pSystemNotificationTypeGuid, Guid pDocumentType, string pExtraFilter, int pDaysBack)
        {
            string filter = string.Format("(DocumentType = '{0}' AND DocumentStatusStatus = 'N')", pDocumentType.ToString());
            return ProcessFinanceDocumentToInvoice(pSession, pSystemNotificationTypeGuid, filter, pExtraFilter, pDaysBack);
        }

        //Overload From SaftDocumentType Enum
        public static sys_systemnotification ProcessFinanceDocumentToInvoice(Session pSession, Guid pSystemNotificationTypeGuid, SaftDocumentType pSaftDocumentType, string pExtraFilter, int pDaysBack)
        {
            string filter = String.Empty;

            CriteriaOperator criteriaOperator = CriteriaOperator.Parse(string.Format("(SaftDocumentType = {0})", Convert.ToInt16(pSaftDocumentType)));
            XPCollection xpcDocumentFinanceType = new XPCollection(pSession, typeof(fin_documentfinancetype), criteriaOperator);
            if (xpcDocumentFinanceType.Count > 0)
            {
                int i = 0;
                foreach (fin_documentfinancetype item in xpcDocumentFinanceType)
                {
                    i++;
                    filter += (string.Format("DocumentType = '{0}'{1}", item.Oid, (i < xpcDocumentFinanceType.Count) ? " OR " : String.Empty));
                }
                filter = string.Format("({0})", filter);
            }

            return ProcessFinanceDocumentToInvoice(pSession, pSystemNotificationTypeGuid, filter, pExtraFilter, pDaysBack);
        }

        //Base Method
        public static sys_systemnotification ProcessFinanceDocumentToInvoice(Session pSession, Guid pSystemNotificationTypeGuid, string pFilter, string pExtraFilter, int pDaysBackToFilter)
        {
            //Init Local Vars
            sys_systemnotificationtype systemNotificationType = (sys_systemnotificationtype)pSession.GetObjectByKey(typeof(sys_systemnotificationtype), pSystemNotificationTypeGuid);
            sys_systemnotification result = null;
            //Used to Persist sys_systemnotification if greater than 0
            int totalNotificatedDocuments = 0;
            //Show total Showed Notification in Document
            int totalNotificationsInDocument = 0;
            //Ignore Notificate Documents after Documents Have Been Notified a determined Number Of Times
            int ignoreNotificationsAfterHaveBeenNotificatedNumberOfTimes = 0;
            try
            {
                ignoreNotificationsAfterHaveBeenNotificatedNumberOfTimes = Convert.ToInt16(GlobalFramework.PreferenceParameters["NOTIFICATION_DOCUMENTS_TO_INVOICE_IGNORE_AFTER_SHOW_NUMBER_OF_TIMES"]);
            }
            catch (Exception)
            {
                _log.Error(string.Format("Error using PreferenceParameters: NOTIFICATION_DOCUMENTS_TO_INVOICE_IGNORE_AFTER_SHOW_NUMBER_OF_TIMES, using default Value of : [{0}]", ignoreNotificationsAfterHaveBeenNotificatedNumberOfTimes));
            }

            try
            {
                //Get Date Back DaysBackToFilter (Without WeekEnds and Holidays)
                int warnDaysBefore = (systemNotificationType.WarnDaysBefore > 0) ? systemNotificationType.WarnDaysBefore : 0;
                int daysBackToFilter = pDaysBackToFilter - warnDaysBefore;
                DateTime dateFilterFrom = GetDateTimeBackUtilDays(CurrentDateTimeAtomicMidnight(), daysBackToFilter, true);

                //Extra Filter 
                string filter = pFilter;
                if (pExtraFilter != String.Empty) filter = string.Format("{0} AND {1}", filter, pExtraFilter);
                filter = string.Format("{0} AND (Date <= '{1} 23:59:59')", filter, dateFilterFrom.ToString(SettingsApp.DateFormat));

                CriteriaOperator criteriaOperator = CriteriaOperator.Parse(filter);
                SortProperty sortProperty = new SortProperty("CreatedAt", DevExpress.Xpo.DB.SortingDirection.Ascending);
                XPCollection xpcDocumentFinanceMaster = new XPCollection(pSession, typeof(fin_documentfinancemaster), criteriaOperator, sortProperty);

                // Debug Helper
                //if (pSystemNotificationTypeGuid.Equals(SettingsApp.XpoOidSystemNotificationTypeSaftDocumentTypeMovementOfGoods))
                //{
                //    _log.Debug("BREAK");
                //}

                if (xpcDocumentFinanceMaster.Count > 0)
                {
                    int i = 0;
                    String documentsMessage = String.Empty;
                    int documentBackUtilDays = 0;

                    //Generate DocumentNumber List documentsMessage
                    foreach (fin_documentfinancemaster item in xpcDocumentFinanceMaster)
                    {
                        i++;
                        //Get BackDays
                        documentBackUtilDays = GetUtilDays(item.Date, true).Count;
                        // Get total notification for Current Document
                        totalNotificationsInDocument = item.Notifications.Count;

                        // If document has less notifications show it again, or if is ignored with ignoreNotificationsAfterBeenNotificatedTimes == 0
                        if (ignoreNotificationsAfterHaveBeenNotificatedNumberOfTimes == 0 || totalNotificationsInDocument < ignoreNotificationsAfterHaveBeenNotificatedNumberOfTimes)
                        {
                            // Increment notifications counter
                            totalNotificatedDocuments++;
                            // Add To Message
                            documentsMessage += string.Format("- {0} : {1} : {2} {3} : (#{4})", item.DocumentNumber, item.Date, documentBackUtilDays, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_day_days"), item.Notifications.Count + 1);
                            //Add New Line if not Last Document
                            if (i < xpcDocumentFinanceMaster.Count) documentsMessage += Environment.NewLine;
                        }
                    }

                    // Create Notification Object if has notifications
                    if (totalNotificatedDocuments > 0)
                    {
                        result = new sys_systemnotification(pSession)
                        {
                            NotificationType = systemNotificationType,
                            Message = string.Format(systemNotificationType.Message, totalNotificatedDocuments, pDaysBackToFilter, documentsMessage)
                        };
                        result.Save();

                        //Persist sys_systemnotificationdocumentmaster manyToMany Relantionship
                        foreach (fin_documentfinancemaster item in xpcDocumentFinanceMaster)
                        {
                            sys_systemnotificationdocumentmaster notificationDocumentMasterReference = new sys_systemnotificationdocumentmaster(pSession)
                            {
                                DocumentMaster = item,
                                Notification = result
                            };
                            notificationDocumentMasterReference.Save();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Validation

        public static bool Validate(string pValidateValue, string pRule, bool pRequired)
        {
            bool result = false;

            try
            {
                //Without Rule
                if (pRule == null || pRule == string.Empty)
                {
                    if (pRequired)
                    {
                        if (pValidateValue == string.Empty)
                        {
                            result = false;
                        }
                        else if (pValidateValue != string.Empty)
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        result = true;
                    }
                }
                //With Rule
                else
                {
                    if (pRequired)
                    {
                        if (pValidateValue != string.Empty && ValidateString(pValidateValue, pRule))
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        //If not Required and as a Empty Value or Empty Guid
                        if (pValidateValue == string.Empty || pValidateValue == Convert.ToString(Guid.Empty))
                        {
                            result = true;
                        }
                        else
                            if (pValidateValue != string.Empty && ValidateString(pValidateValue, pRule))
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        public static ValidateMaxLenghtMaxWordsResult ValidateMaxLenghtMaxWords(string pValue, string pIinitialLabelText, int pMaxLength, int pMaxWords)
        {
            ValidateMaxLenghtMaxWordsResult result = new ValidateMaxLenghtMaxWordsResult();

            try
            {
                result.Text = pValue;
                result.Length = pValue.Length;

                string lengthLabelText = string.Empty;
                string maxWordsLabelText = string.Empty;

                if (pMaxLength > 0)
                {
                    if (result.Length > pMaxLength)
                    {
                        result.Length = pMaxLength;
                        //Cut Text
                        result.Text = pValue.Substring(0, pMaxLength);
                    }
                    lengthLabelText = string.Format("{0}: {1}/{2}", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_characters"), result.Length, pMaxLength);
                }

                result.Words = GetNumWords(result.Text);

                if (pMaxWords > 0)
                {
                    if (result.Words > pMaxWords)
                    {
                        result.Words = pMaxWords;
                        result.Text = GetWords(result.Text, pMaxWords);
                    }
                    maxWordsLabelText = string.Format("{0}: {1}/{2}", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_words"), result.Words, pMaxWords);
                }

                if (result.Length > 0)
                {
                    string addToLabelText = string.Empty;
                    if (lengthLabelText != string.Empty) addToLabelText += lengthLabelText;
                    if (lengthLabelText != string.Empty && maxWordsLabelText != string.Empty) addToLabelText += " ";
                    if (maxWordsLabelText != string.Empty) addToLabelText += maxWordsLabelText;

                    if (addToLabelText != string.Empty)
                    {
                        result.LabelText = string.Format("{0} : ({1})", pIinitialLabelText, addToLabelText);
                    }
                }
                else
                {
                    result.LabelText = pIinitialLabelText;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //RegEx Validation : ex mail @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"

        public static bool ValidateString(string pValidate, string pRegExRule, Type pType = null)
        {
            //If Type Decimal and User uses "," replace it by "."
            if (pType == typeof(decimal)) pValidate = pValidate.Replace(',', '.');

            //Check if can Convert to type
            if (pType != null && !CanConvert(pValidate, pType))
            {
                return false;
            };

            //Check if is a Blank GUID 
            if (pValidate == new Guid().ToString() && pRegExRule == SettingsApp.RegexGuid)
            {
                return false;
            }

            if (pValidate != string.Empty && pRegExRule != string.Empty)
            {
                try
                {
                    return Regex.IsMatch(pValidate, pRegExRule);
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool CanConvert(string pValue, Type pType)
        {
            if (string.IsNullOrEmpty(pValue) || pType == null) return false;
            System.ComponentModel.TypeConverter typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(pType);
            if (typeConverter.CanConvertFrom(typeof(string)))
            {
                try
                {
                    //typeConverter.ConvertFrom(pValue);
                    typeConverter.ConvertFrom(null, GlobalFramework.CurrentCultureNumberFormat, pValue);
                    return true;
                }
                catch (Exception)
                {
                    //Hide Error from log
                    //_log.Error(ex.Message, ex);
                    return false;
                }
            }
            return false;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //WebServices

        //Get <endpoint address="http://localhost:59735/Service1.svc" binding="basicHttpBinding"
        //ex http://localhost:59735/Service1.svc
        public static string GetEndpointAddress()
        {
            string result = string.Empty;

            try
            {
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
                ServiceModelSectionGroup serviceModelSectionGroup = ServiceModelSectionGroup.GetSectionGroup(configuration);
                ClientSection clientSection = serviceModelSectionGroup.Client;
                var el = clientSection.Endpoints[0];
                result = el.Address.ToString();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        public static bool IsWebServiceOnline(string pUrl)
        {
            bool result = false;

            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(pUrl);
                httpReq.AllowAutoRedirect = false;
                HttpWebResponse httpRes = (HttpWebResponse)httpReq.GetResponse();
                if (httpRes.StatusCode != HttpStatusCode.OK)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }

                httpRes.Close();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Reflection

        //Get Property value from String
        public static object GetPropValue(object pObject, string pProperty)
        {
            return pObject.GetType().GetProperty(pProperty).GetValue(pObject, null);
        }

        public static object GetPropertyValueFromType(Type pType, string pPropertyName)
        {
            object result = null;

            try
            {
                // Get a PropertyInfo of specific property type(T).GetProperty(....)
                PropertyInfo propertyInfo = pType.GetProperty(pPropertyName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                // Use the PropertyInfo to retrieve the value from the type by not passing in an instance
                // Note: use first parameter has null, static classes cannot be instanced, else use object
                result = propertyInfo.GetValue(null, null);

                //PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                //foreach (PropertyInfo property in properties)
                //{
                //    _log.Debug(String.Format("[{0}] == [{1}]", property.Name, pFieldName));
                //}
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        public static object GetFieldValueFromType(Type pType, string pFieldName)
        {
            object result = null;

            try
            {
                //pFieldName = "DatabaseName";//Works with Current Settings
                //pFieldName = "AppCompanyName";//Works with Base Class
                //object o = pType.BaseType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.FlattenHierarchy);

                //Trick to get current and base class Fields
                FieldInfo fieldInfo = pType.GetField(pFieldName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                //Note: use first parameter has null, static classes cannot be instanced, else use object
                if (fieldInfo != null) result = fieldInfo.GetValue(null);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // SoftwareVendor

        public static string GetSoftwareVendorValueAsString(string property, bool debug = false)
        {
            return Convert.ToString(GetPluginSoftwareVendorValue(property, debug));
        }

        public static int GetSoftwareVendorValueAsInt(string property, bool debug = false)
        {
            return Convert.ToInt16(GetPluginSoftwareVendorValue(property, debug));
        }

        public static bool GetSoftwareVendorValueAsBool(string property, bool debug = false)
        {
            return Convert.ToBoolean(GetPluginSoftwareVendorValue(property, debug));
        }

        public static object GetPluginSoftwareVendorValue(string property, bool debug = false)
        {
            object resultObject = null;

            if (GlobalFramework.PluginSoftwareVendor != null)
            {
                Type thisType = GlobalFramework.PluginSoftwareVendor.GetType();
                string methodName = string.Format("Get{0}", property);
                MethodInfo methodInfo = thisType.GetMethod(methodName);
                object[] methodParameters = null;
                resultObject = methodInfo.Invoke(GlobalFramework.PluginSoftwareVendor, methodParameters);
            }

            if (debug) _log.Debug(String.Format("SoftwareVendor {0} Value: [{1}]", property, resultObject));

            if (resultObject != null)
            {
                return resultObject;
            }
            else
            {
                return null;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Words | Medianova

        private static string PrepareCutWord(string pText)
        {
            //Init Local Vars
            string tmpText = pText;

            tmpText = tmpText.Replace('.', ' ');
            tmpText = tmpText.Replace(',', ' ');
            tmpText = tmpText.Replace(';', ' ');
            tmpText = tmpText.Replace(':', ' ');
            tmpText = tmpText.Replace('+', ' ');
            tmpText = tmpText.Replace('/', ' ');
            tmpText = tmpText.Replace('$', ' ');
            tmpText = tmpText.Replace('=', ' ');
            tmpText = tmpText.Replace('#', ' ');
            tmpText = tmpText.Replace('"', ' ');
            tmpText = tmpText.Replace('!', ' ');

            return (tmpText);
        }

        //Refactored to be equal to JavaScript Function Names
        //Mario changes from "private" to "public static" to access outside, ex from components
        //public static int CountWords(string pText)
        public static int GetNumWords(string pText)
        {
            int result = 0;
            int minWordLengthConsidered = Convert.ToInt16(GlobalFramework.Settings["MinWordLengthConsidered"]);

            //Require to pass string to PreparedCutWord 
            string text = PrepareCutWord(pText);

            text = text.Replace("\n", " ");
            string[] res = text.Split(' ');
            for (int i = 0; i < res.Length; i++)
            {
                if (res[i] != "")
                {
                    if (res[i].Length >= minWordLengthConsidered)
                    {
                        result++;
                    }
                }
            }

            return (result);
        }

        private string CutWords(string pText, int pMaxWords)
        {
            //Init Local Vars
            string result = "";
            int tmpCounter = 0;
            bool tmpCutted = false;
            int minWordLengthConsidered = Convert.ToInt16(GlobalFramework.Settings["MinWordLengthConsidered"]);

            //Require to pass string to PreparedCutWord 
            string text = PrepareCutWord(pText);

            if (pMaxWords == 0)
            {
                result = text;
            }
            else if (text != null)
            {
                string[] res = text.Split(' ');
                for (int i = 0; i < res.Length; i++)
                {
                    if (res[i] != "")
                    {
                        if (res[i].Length >= minWordLengthConsidered)
                        {
                            if (tmpCounter < pMaxWords)
                            {
                                result += res[i] + " ";
                                tmpCounter++;
                            }
                            else
                            {
                                result = text.Substring(0, text.IndexOf(res[i], result.Length));
                                tmpCutted = true;
                                break;
                            }
                        }
                        else
                        {
                            result += res[i];
                        }
                    }
                }
            }

            if (tmpCutted != true)
            {
                result = text;
            }

            return (result);
        }

        public static string GetWords(string pText, int pNoWords)
        {
            string result = string.Empty;

            int minWordLengthConsidered = Convert.ToInt16(GlobalFramework.Settings["MinWordLengthConsidered"]);

            pText = pText.Replace("\n", " ");
            pText = pText.Replace(".", " ");
            pText = pText.Replace(",", " ");
            pText = pText.Replace(";", " ");
            pText = pText.Replace(":", " ");
            pText = pText.Replace("+", " ");
            pText = pText.Replace("/", " ");
            pText = pText.Replace("$", " ");
            pText = pText.Replace("=", " ");
            pText = pText.Replace("#", " ");
            pText = pText.Replace("\\", " ");
            pText = pText.Replace("!", " ");
            string[] res = pText.Split(' ');
            for (int i = 0; i < pNoWords; i++)
            {
                result += res[i];
                if (i < pNoWords - 1) result += " ";
            }

            return (result);
        }
    }
}

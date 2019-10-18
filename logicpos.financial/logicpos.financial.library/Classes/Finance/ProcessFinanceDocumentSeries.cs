using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.App;
using logicpos.financial.library.Results;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace logicpos.financial.library.Classes.Finance
{
    public class ProcessFinanceDocumentSeries
    {
        //Log4Net
        protected static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected static bool _debug = false;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //GetDocumentFinanceYearSerieTerminal

        //Get DocumentFinanceYearSerieTerminal for Logged Terminal
        public static fin_documentfinanceyearserieterminal GetDocumentFinanceYearSerieTerminal(Guid pDocumentType)
        {
            return GetDocumentFinanceYearSerieTerminal(GlobalFramework.SessionXpo, pDocumentType, GlobalFramework.LoggedTerminal.Oid);
        }

        public static fin_documentfinanceyearserieterminal GetDocumentFinanceYearSerieTerminal(Session pSession, Guid pDocumentType)
        {
            return GetDocumentFinanceYearSerieTerminal(pSession, pDocumentType, GlobalFramework.LoggedTerminal.Oid);
        }

        //Get DocumentFinanceYearSerieTerminal for Terminal
        public static fin_documentfinanceyearserieterminal GetDocumentFinanceYearSerieTerminal(Session pSession, Guid pDocumentType, Guid pLoggedTerminal)
        {
            DateTime currentDateTime = FrameworkUtils.CurrentDateTimeAtomic();
            fin_documentfinanceyearserieterminal documentFinanceYearSerieTerminal = null;
            fin_documentfinancetype documentFinanceType = (fin_documentfinancetype)FrameworkUtils.GetXPGuidObject(pSession, typeof(fin_documentfinancetype), pDocumentType);

            //If DocumentTypeInvoiceWayBill Replace/Override Helper Document Type InvoiceWayBill with InvoiceWay to get Invoice Serie, 
            //this way we have Invoice Serie but DocumentMaster keeps DocumentFinanceType has DocumentFinanceTypeInvoiceWayBill
            //Usefull for Future Documents WayBill distinct code, ex have WayBill, ex Re-Print Documents in WayBillMode etc
            Guid documentFinanceTypeSerieGuid = (documentFinanceType.Oid == SettingsApp.XpoOidDocumentFinanceTypeInvoiceWayBill)
                ? SettingsApp.XpoOidDocumentFinanceTypeInvoice
                : documentFinanceType.Oid
            ;

            //Get Document Serie
            SortingCollection sortCollection = new SortingCollection();
            sortCollection.Add(new SortProperty("FiscalYear", DevExpress.Xpo.DB.SortingDirection.Ascending));
            CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND DocumentType == '{0}'", documentFinanceTypeSerieGuid.ToString()));
            ICollection collectionDocumentFinanceSeries = pSession.GetObjects(pSession.GetClassInfo(typeof(fin_documentfinanceyearserieterminal)), criteria, sortCollection, int.MaxValue, false, true);

            foreach (fin_documentfinanceyearserieterminal item in collectionDocumentFinanceSeries)
            {
                if (currentDateTime.Year >= item.FiscalYear.FiscalYear)
                {
                    if (item.Terminal.Oid == pLoggedTerminal)
                    {
                        documentFinanceYearSerieTerminal = item;
                    }
                }
            }

            return documentFinanceYearSerieTerminal;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Get Current Active FinanceYear

        public static fin_documentfinanceyears GetCurrentDocumentFinanceYear()
        {
            fin_documentfinanceyears result = null;

            string sql = @"SELECT Oid FROM fin_documentfinanceyears WHERE (Disabled = 0 OR Disabled IS NULL);";
            var sqlResult = GlobalFramework.SessionXpo.ExecuteScalar(sql);
            if (sqlResult != null)
            {
                result = (fin_documentfinanceyears)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_documentfinanceyears), new Guid(sqlResult.ToString()));
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Check if has Active Document Finance Series, can have a filter parameter to check for Individual Series ex with DocumentType and FiscalYear

        /* NOT Used
        public static bool HasActiveDocumentFinanceSeries()
        {
            return HasActiveDocumentFinanceSeries(String.Empty);
        }

        public static bool HasActiveDocumentFinanceSeries(string pFilter)
        {
            bool result = false;
            if (pFilter != String.Empty) pFilter = string.Format(" AND {0}", pFilter);
            string sql = string.Format(@"SELECT Count(*) as Count FROM fin_documentfinanceseries WHERE (Disabled = 0 OR Disabled IS NULL){0};", pFilter);
            var sqlResult = GlobalFramework.SessionXpo.ExecuteScalar(sql);
            if (sqlResult != null && Convert.ToInt16(sqlResult) > 0) result = true;
            return result;
        }
        */

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper for Disable All Year Related Series and Series Terminal

        public static bool DisableActiveYearSeriesAndTerminalSeries(fin_documentfinanceyears pDocumentFinanceYears)
        {
            bool result = false;
            //Used to add DocumentFinanceYearSerieTerminal list to disable outside of Loop
            List<fin_documentfinanceseries> listDisableSeries = new List<fin_documentfinanceseries>();
            //Used to add DocumentFinanceYearSerieTerminal to list to delete outside of Loop
            List<fin_documentfinanceyearserieterminal> listDeleteSerieTerminal = new List<fin_documentfinanceyearserieterminal>();

            //Start UnitOfWork
            using (UnitOfWork uowSession = new UnitOfWork())
            {
                try
                {
                    //Get Object in UOW Session
                    fin_documentfinanceyears documentFinanceYears = (fin_documentfinanceyears)FrameworkUtils.GetXPGuidObject(uowSession, typeof(fin_documentfinanceyears), pDocumentFinanceYears.Oid);

                    //Protection, used when user Restore DB without DocumentFinanceYears Created
                    if (documentFinanceYears != null)
                    {
                        foreach (fin_documentfinanceseries documentFinanceSeries in documentFinanceYears.Series)
                        {
                            if (!documentFinanceSeries.Disabled)
                            {
                                listDisableSeries.Add(documentFinanceSeries);
                            }
                            foreach (fin_documentfinanceyearserieterminal documentFinanceYearSerieTerminal in documentFinanceSeries.YearSerieTerminal)
                            {
                                listDeleteSerieTerminal.Add(documentFinanceYearSerieTerminal);
                            }
                        }

                        //Now we can disable, Outside of Loop
                        foreach (fin_documentfinanceseries disableSerie in listDisableSeries)
                        {
                            if (_debug) _log.Debug(string.Format("  {0} : Disabled:[{1}]", disableSerie.Designation, disableSerie.Disabled));
                            disableSerie.Disabled = true;
                        }
                        //Now we can delete, Outside of Loop
                        foreach (fin_documentfinanceyearserieterminal deleteSerieTerminal in listDeleteSerieTerminal)
                        {
                            if (_debug) _log.Debug(string.Format("    {0} : Deleted:[{1}]", deleteSerieTerminal.Designation, deleteSerieTerminal.Disabled));
                            deleteSerieTerminal.Delete();
                        }

                        //Finnaly Commit Changes
                        uowSession.CommitChanges();
                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    uowSession.RollbackTransaction();
                    _log.Error(ex.Message, ex);
                }
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper for Create DocumentFinanceSeries & DocumentFinanceYearSerieTerminal
        //Used to generate Series Preview, or to Process Series in Database

        public static FrameworkCallsResult CreateDocumentFinanceYearSeriesTerminal(fin_documentfinanceyears pDocumentFinanceYears, DataTable pTerminals, string pAcronym, bool pPreviewMode)
        {
            FrameworkCallsResult result = new FrameworkCallsResult();
            uint ordAndCode = 10;
            uint ordAndCodeInc = 10;
            int terminalInc = 1;
            string acronymPrefix = String.Empty, acronym, designation, output = String.Empty, acronymAudit;
            Dictionary<string,string> acronymPrefixCreatedSeries = new Dictionary<string,string>();
            fin_documentfinanceseries documentFinanceSeries = null;
            //Used to add DocumentFinanceYearSerieTerminal to list to delete outside of Loop
            List<fin_documentfinanceyearserieterminal> listDeleteSerieTerminal = new List<fin_documentfinanceyearserieterminal>();

            //Start UnitOfWork
            using (UnitOfWork uowSession = new UnitOfWork())
            {
                try
                {
                    //Get Object in UOW Session
                    fin_documentfinanceyears documentFinanceYears = (fin_documentfinanceyears)FrameworkUtils.GetXPGuidObject(uowSession, typeof(fin_documentfinanceyears), pDocumentFinanceYears.Oid);

                    //Initialize DocumentFinanceType Collection : Criteria/XPCollection/Model : Use Default Filter
                    CriteriaOperator criteria = CriteriaOperator.Parse("(Disabled = 0 OR Disabled IS NULL)");
                    //Configure SortProperty
                    SortProperty sortProperty = new SortProperty("Ord", DevExpress.Xpo.DB.SortingDirection.Ascending);
                    //Init Collection
                    XPCollection xpDocumentFinanceType = new XPCollection(uowSession, typeof(fin_documentfinancetype), criteria, sortProperty);

                    //Loop Terminals
                    foreach (DataRow terminal in pTerminals.Rows)
                    {
                        //Disable Series and Delete SerieTerminal for selected Terminal
                        if (!pPreviewMode)
                        {
                            //Initialize DocumentFinanceYearSerieTerminal Collection : Criteria/XPCollection/Model : Use Default Filter + Terminal
                            criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND (Terminal = '{0}')", terminal["Oid"]));
                            //Init Collection
                            XPCollection xpDocumentFinanceYearSerieTerminal = new XPCollection(uowSession, typeof(fin_documentfinanceyearserieterminal), criteria);

                            //Loop DocumentFinanceYearSerieTerminal and Parent DocumentFinanceYearSerie and Delete 
                            foreach (fin_documentfinanceyearserieterminal documentFinanceYearSerieTerminal in xpDocumentFinanceYearSerieTerminal)
                            {
                                if (_debug) _log.Debug(string.Format("Disabled documentFinanceYearSerie: [{0}]", documentFinanceYearSerieTerminal.Serie.Designation));
                                documentFinanceYearSerieTerminal.Serie.Disabled = true;
                                //documentFinanceYearSerieTerminal.Disabled = true;
                                //Add to Post Loop Deletion
                                listDeleteSerieTerminal.Add(documentFinanceYearSerieTerminal);
                            }

                            //Now we can delete, Outside of Loop
                            foreach (fin_documentfinanceyearserieterminal deleteSerieTerminal in listDeleteSerieTerminal)
                            {
                                if (_debug) _log.Debug(string.Format("Deleted documentFinanceYearSerieTerminal: [{0}]", deleteSerieTerminal.Designation));
                                deleteSerieTerminal.Delete();
                            }
                        }

                        //Add to Output
                        if (pPreviewMode) output += string.Format("{0}{1}", terminal["Designation"], Environment.NewLine);

                        //Get Current Terminal Object
                        pos_configurationplaceterminal configurationPlaceTerminal = (pos_configurationplaceterminal)uowSession.GetObjectByKey(typeof(pos_configurationplaceterminal), new Guid(terminal["Oid"].ToString()));

                        //Create DocumentFinanceSeries Acronym From Date
                        DateTime now = FrameworkUtils.CurrentDateTimeAtomic();

                        //AcronymPrefix ex FT[QN3T1U401]2016S001, works with Random and AcronymLastSerie modes
                        if (SettingsApp.DocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix)
                        {
                            acronymPrefix = DateToAcronymPrefix(new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second));
                        }
                        //AcronymPrefix ex Fatura FT[0001]2016S001
                        else
                        {
                            //Get acronymPrefix in first DocumentFinanceType, not in every Document, this way we have uniform series
                            acronymPrefix = (xpDocumentFinanceType[0] as fin_documentfinancetype).AcronymLastSerie.ToString(SettingsApp.DocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat);
                        }

                        //Add to Created List
                        acronymPrefixCreatedSeries.Add(acronymPrefix, configurationPlaceTerminal.Designation);

                        foreach (fin_documentfinancetype documentFinanceType in xpDocumentFinanceType)
                        {
                            //Ignored DocumentTypes (DocumentFinanceTypeInvoiceWayBill, this DocumentType use DocumentFinanceTypeInvoice Serie)
                            if (documentFinanceType.Oid != SettingsApp.XpoOidDocumentFinanceTypeInvoiceWayBill)
                            {
                                //2018-05-08 : Old Format : [FT005012018S1] : Search GenDocumentNumber in ProcessFinanceDocument
                                //acronym = string.Format("{0}{1}{2}{3}", documentFinanceType.Acronym, acronymPrefix, terminalInc.ToString("00"), pAcronym);
                                //2018-05-08 : New Format
                                //acronym = string.Format("{0} {1}{2}{3}", documentFinanceType.Acronym, pAcronym, acronymPrefix, terminalInc);
                                acronym = string.Format("{0} {1}{2}", documentFinanceType.Acronym, pAcronym, acronymPrefix);
                                designation = string.Format("{0} {1}", documentFinanceType.Designation, acronym);
                                if (!pPreviewMode)
                                {
                                    documentFinanceSeries = new fin_documentfinanceseries(uowSession)
                                    {
                                        Ord = ordAndCode,
                                        Code = ordAndCode,
                                        FiscalYear = documentFinanceYears,
                                        DocumentType = documentFinanceType,
                                        NextDocumentNumber = 1,
                                        DocumentNumberRangeBegin = 1,
                                        DocumentNumberRangeEnd = int.MaxValue,
                                        Acronym = acronym,
                                        Designation = designation
                                    };
                                }
                                //Add to Output
                                if (pPreviewMode) output += string.Format("  {0} {1} - {2}{3}", documentFinanceType.Acronym, acronym, documentFinanceType.Designation, Environment.NewLine);
                                if (_debug) _log.Debug(string.Format("DocumentFinanceSeries: [{0}], Designation: [{1}], Acronym: [{2}]", ordAndCode, designation, acronym));

                                //Create DocumentFinanceYearSerieTerminal
                                designation = string.Format("{0} {1}", designation, configurationPlaceTerminal.Designation);
                                if (!pPreviewMode)
                                {
                                    fin_documentfinanceyearserieterminal documentFinanceYearSerieTerminal = new fin_documentfinanceyearserieterminal(uowSession)
                                    {
                                        Ord = ordAndCode,
                                        Code = ordAndCode,
                                        FiscalYear = documentFinanceYears,
                                        DocumentType = documentFinanceSeries.DocumentType,
                                        Serie = documentFinanceSeries,
                                        Terminal = configurationPlaceTerminal,
                                        Printer = configurationPlaceTerminal.Printer,
                                        Template = documentFinanceSeries.DocumentType.Template,
                                        Designation = string.Format("{0} {1}", documentFinanceSeries.Designation, configurationPlaceTerminal.Designation)
                                    };
                                }
                                if (_debug) _log.Debug(string.Format("DocumentFinanceYearSerieTerminal: [{0}], Terminal: [{1}], Serie: [{2}]", ordAndCode, terminal["Designation"], designation));

                                //Increment AcronymLastSerie and ordAndCodeInc
                                documentFinanceType.AcronymLastSerie++;
                                ordAndCode = ordAndCode + ordAndCodeInc;
                            }
                        }
                        //Add Blank Line to Split Series/Terminal
                        if (pPreviewMode && pTerminals.Rows.Count > terminalInc) output += Environment.NewLine;
                        //Increment Terminal
                        terminalInc++;
                    }

                    //Finnaly Commit Changes
                    if (!pPreviewMode)
                    {
                        uowSession.CommitChanges();
                    }

                    result.Result = true;

                    if (pPreviewMode)
                    {
                        result.Output = output;
                    }
                    else
                    {
                        foreach (var item in acronymPrefixCreatedSeries)
                        {
                            //Audit FINANCE_SERIES_CREATED
                            acronymAudit = string.Format("{0}{1}{2}{3}", "xx", item.Key, 0.ToString("00"), pAcronym);
                            FrameworkUtils.Audit("FINANCE_SERIES_CREATED", string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "audit_message_finance_series_created"), acronymAudit, item.Value, GlobalFramework.LoggedUser.Name));
                        }
                    }
                }
                catch (Exception ex)
                {
                    uowSession.RollbackTransaction();
                    result.Exception = ex;
                    _log.Error(ex.Message, ex);
                }
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        //Private Members for Acronym Series Methods
        private static string _acronymDateTimeFormat = "{2}:{1}:{0} {3}/{4}/YYYY";
        private static string _base36Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        //Generate Random Acronym Based on Hash Date
        private static string DateToAcronymPrefix(DateTime pDateTime)
        {
            string result = String.Empty;
            DateTime currentTime = pDateTime;
            string date = string.Format("{0}{1}{2}{3}{4}", currentTime.Second.ToString("00"), currentTime.Minute.ToString("00"), currentTime.Hour.ToString("00"), currentTime.Day.ToString("00"), currentTime.Month.ToString("00"));
            //#1 - Always Add First 1;
            date = string.Format("1{0}", date);
            long uintValue = Convert.ToInt64(date);
            //To Base36
            return Base10To36(uintValue);
        }

        private static DateTime AcronymPrefixToDate(int pYear, string pInput)
        {
            try
            {
                long baseX = Base36To10(pInput);
                string uintString = Convert.ToString(baseX);
                //#1 - Always Remove First 1;
                uintString = uintString.Substring(1, uintString.Length - 1);

                string date = string.Format(
                    _acronymDateTimeFormat,
                    uintString.Substring(0 * 2, 2),//hour
                    uintString.Substring(1 * 2, 2),//minute
                    uintString.Substring(2 * 2, 2),//second
                    uintString.Substring(3 * 2, 2),//day
                    uintString.Substring(4 * 2, 2) //month
                );

                date = date.Replace("YYYY", pYear.ToString());

                return Convert.ToDateTime(date);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                return new DateTime();
            }
        }

        private static string Base10To36(long N)
        {
            string R = "";
            while (N != 0)
            {
                R += _base36Chars[(int)(N % 36)];
                N /= 36;
            }
            return R;
        }

        private static long Base36To10(string N)
        {
            long R = 0;
            int L = N.Length;
            for (int i = 0; i < L; i++)
            {
                R += _base36Chars.IndexOf(N[i]) * (long)Math.Pow(36, i);
            }

            return R;
        }
    }
}

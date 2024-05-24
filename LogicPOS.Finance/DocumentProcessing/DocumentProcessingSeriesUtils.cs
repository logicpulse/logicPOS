using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using LogicPOS.DTOs.Common;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace LogicPOS.Finance.DocumentProcessing
{
    public static class DocumentProcessingSeriesUtils
    {
        //Log4Net
        private static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static bool _debug = false;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //GetDocumentFinanceYearSerieTerminal

        //Get DocumentFinanceYearSerieTerminal for Logged Terminal
        public static fin_documentfinanceyearserieterminal GetDocumentFinanceYearSerieTerminal(Guid pDocumentType)
        {
            return GetDocumentFinanceYearSerieTerminal(XPOSettings.Session, pDocumentType, XPOSettings.LoggedTerminal.Oid);
        }

        public static fin_documentfinanceyearserieterminal GetDocumentFinanceYearSerieTerminal(Session pSession, Guid pDocumentType)
        {
            return GetDocumentFinanceYearSerieTerminal(pSession, pDocumentType, XPOSettings.LoggedTerminal.Oid);
        }

        //Get DocumentFinanceYearSerieTerminal for Terminal
        public static fin_documentfinanceyearserieterminal GetDocumentFinanceYearSerieTerminal(Session pSession, Guid pDocumentType, Guid pLoggedTerminal)
        {
            DateTime currentDateTime = XPOHelper.CurrentDateTimeAtomic();
            fin_documentfinanceyearserieterminal documentFinanceYearSerieTerminal = null;
            fin_documentfinancetype documentFinanceType = (fin_documentfinancetype)XPOHelper.GetXPGuidObject(pSession, typeof(fin_documentfinancetype), pDocumentType);

            //If DocumentTypeInvoiceWayBill Replace/Override Helper Document Type InvoiceWayBill with InvoiceWay to get Invoice Serie, 
            //this way we have Invoice Serie but DocumentMaster keeps DocumentFinanceType has DocumentFinanceTypeInvoiceWayBill
            //Usefull for Future Documents WayBill distinct code, ex have WayBill, ex Re-Print Documents in WayBillMode etc
            Guid documentFinanceTypeSerieGuid = (documentFinanceType.Oid == DocumentSettings.XpoOidDocumentFinanceTypeInvoiceWayBill)
                ? InvoiceSettings.XpoOidDocumentFinanceTypeInvoice
                : documentFinanceType.Oid
            ;

            //Get Document Serie
            SortingCollection sortCollection = new SortingCollection
            {
                new SortProperty("FiscalYear", DevExpress.Xpo.DB.SortingDirection.Ascending)
            };
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
            var sqlResult = XPOSettings.Session.ExecuteScalar(sql);
            if (sqlResult != null)
            {
                result = (fin_documentfinanceyears)XPOSettings.Session.GetObjectByKey(typeof(fin_documentfinanceyears), new Guid(sqlResult.ToString()));
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
            var sqlResult = XPOSettings.Session.ExecuteScalar(sql);
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
                    fin_documentfinanceyears documentFinanceYears = (fin_documentfinanceyears)XPOHelper.GetXPGuidObject(uowSession, typeof(fin_documentfinanceyears), pDocumentFinanceYears.Oid);

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
                            if (_debug) _logger.Debug(string.Format("  {0} : Disabled:[{1}]", disableSerie.Designation, disableSerie.Disabled));
                            disableSerie.Disabled = true;
                        }
                        //Now we can delete, Outside of Loop
                        foreach (fin_documentfinanceyearserieterminal deleteSerieTerminal in listDeleteSerieTerminal)
                        {
                            if (_debug) _logger.Debug(string.Format("    {0} : Deleted:[{1}]", deleteSerieTerminal.Designation, deleteSerieTerminal.Disabled));
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
                    _logger.Error(ex.Message, ex);
                }
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper for Create DocumentFinanceSeries & DocumentFinanceYearSerieTerminal
        //Used to generate Series Preview, or to Process Series in Database

        public static FrameworkCallResult CreateDocumentFinanceYearSeriesTerminal(fin_documentfinanceyears pDocumentFinanceYears, DataTable pTerminals, string pAcronym, bool pPreviewMode)
        {
            FrameworkCallResult result = new FrameworkCallResult();
            uint ordAndCode = 10;
            uint ordAndCodeInc = 10;
            int terminalInc = 1;
            string acronym, designation, output = string.Empty, acronymAudit;
            Dictionary<string, string> acronymPrefixCreatedSeries = new Dictionary<string, string>();
            fin_documentfinanceseries documentFinanceSeries = null;
            //Used to add DocumentFinanceYearSerieTerminal to list to delete outside of Loop
            List<fin_documentfinanceyearserieterminal> listDeleteSerieTerminal = new List<fin_documentfinanceyearserieterminal>();

            //Start UnitOfWork
            using (UnitOfWork uowSession = new UnitOfWork())
            {
                try
                {
                    //Get Object in UOW Session
                    fin_documentfinanceyears documentFinanceYears = (fin_documentfinanceyears)XPOHelper.GetXPGuidObject(uowSession, typeof(fin_documentfinanceyears), pDocumentFinanceYears.Oid);

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
                                if (_debug) _logger.Debug(string.Format("Disabled documentFinanceYearSerie: [{0}]", documentFinanceYearSerieTerminal.Serie.Designation));
                                documentFinanceYearSerieTerminal.Serie.Disabled = true;
                                //documentFinanceYearSerieTerminal.Disabled = true;
                                //Add to Post Loop Deletion
                                listDeleteSerieTerminal.Add(documentFinanceYearSerieTerminal);
                            }

                            //Now we can delete, Outside of Loop
                            foreach (fin_documentfinanceyearserieterminal deleteSerieTerminal in listDeleteSerieTerminal)
                            {
                                if (_debug) _logger.Debug(string.Format("Deleted documentFinanceYearSerieTerminal: [{0}]", deleteSerieTerminal.Designation));
                                deleteSerieTerminal.Delete();
                            }
                        }

                        //Add to Output
                        if (pPreviewMode) output += string.Format("{0}{1}", terminal["Designation"], Environment.NewLine);

                        //Get Current Terminal Object
                        pos_configurationplaceterminal configurationPlaceTerminal = (pos_configurationplaceterminal)uowSession.GetObjectByKey(typeof(pos_configurationplaceterminal), new Guid(terminal["Oid"].ToString()));

                        //Create DocumentFinanceSeries Acronym From Date
                        DateTime now = XPOHelper.CurrentDateTimeAtomic();
                        string acronymPrefix;
                        //AcronymPrefix ex FT[QN3T1U401]2016S001, works with Random and AcronymLastSerie modes
                        if (DocumentSettings.DocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix)
                        {
                            acronymPrefix = DateToAcronymPrefix(new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second));
                        }
                        //AcronymPrefix ex Fatura FT[0001]2016S001
                        else
                        {
                            //Get acronymPrefix in first DocumentFinanceType, not in every Document, this way we have uniform series
                            acronymPrefix = (xpDocumentFinanceType[0] as fin_documentfinancetype).AcronymLastSerie.ToString(DocumentSettings.DocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat);
                        }

                        //Add to Created List
                        acronymPrefixCreatedSeries.Add(acronymPrefix, configurationPlaceTerminal.Designation);

                        foreach (fin_documentfinancetype documentFinanceType in xpDocumentFinanceType)
                        {
                            //Ignored DocumentTypes (DocumentFinanceTypeInvoiceWayBill, this DocumentType use DocumentFinanceTypeInvoice Serie)
                            if (documentFinanceType.Oid != DocumentSettings.XpoOidDocumentFinanceTypeInvoiceWayBill)
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
                                if (_debug) _logger.Debug(string.Format("DocumentFinanceSeries: [{0}], Designation: [{1}], Acronym: [{2}]", ordAndCode, designation, acronym));

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
                                if (_debug) _logger.Debug(string.Format("DocumentFinanceYearSerieTerminal: [{0}], Terminal: [{1}], Serie: [{2}]", ordAndCode, terminal["Designation"], designation));

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
                            XPOHelper.Audit("FINANCE_SERIES_CREATED", string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "audit_message_finance_series_created"), acronymAudit, item.Value, XPOSettings.LoggedUser.Name));
                        }
                    }
                }
                catch (Exception ex)
                {
                    uowSession.RollbackTransaction();
                    result.Exception = ex;
                    _logger.Error(ex.Message, ex);
                }
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        //Private Members for Acronym Series Methods
        private static readonly string _acronymDateTimeFormat = "{2}:{1}:{0} {3}/{4}/YYYY";
        private static readonly string _base36Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        //Generate Random Acronym Based on Hash Date
        private static string DateToAcronymPrefix(DateTime pDateTime)
        {
            DateTime currentTime = pDateTime;
            string date = string.Format("{0}{1}{2}{3}{4}", currentTime.Second.ToString("00"), currentTime.Minute.ToString("00"), currentTime.Hour.ToString("00"), currentTime.Day.ToString("00"), currentTime.Month.ToString("00"));
            //#1 - Always Add First 1;
            date = string.Format("1{0}", date);
            long uintValue = Convert.ToInt64(date);
            //To Base36
            return Base10To36(uintValue);
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

using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.App;
using logicpos.financial.library.Classes.Reports.BOs.Documents;
using logicpos.resources.Resources.Localization;
using System;
using System.Reflection;

namespace logicpos.financial.library.Classes.Reports.BOs
{
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Generic Collection Result for FRBODocumentFinanceMaster

    public class ResultFRBODocumentFinanceMaster
    {
        private FRBOGenericCollection<FRBODocumentFinanceMasterView> _documentFinanceMaster;
        public FRBOGenericCollection<FRBODocumentFinanceMasterView> DocumentFinanceMaster
        {
            get { return _documentFinanceMaster; }
            set { _documentFinanceMaster = value; }
        }

        public ResultFRBODocumentFinanceMaster()
        {
        }

        public ResultFRBODocumentFinanceMaster(FRBOGenericCollection<FRBODocumentFinanceMasterView> pFRBODocumentFinanceMaster)
        {
            _documentFinanceMaster = pFRBODocumentFinanceMaster;
        }
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Generic Collection Result for FRBODocumentFinancePayment 

    public class ResultFRBODocumentFinancePayment
    {
        private FRBOGenericCollection<FRBODocumentFinancePaymentView> _documentFinancePayment;
        public FRBOGenericCollection<FRBODocumentFinancePaymentView> DocumentFinancePayment
        {
            get { return _documentFinancePayment; }
            set { _documentFinancePayment = value; }
        }

        public ResultFRBODocumentFinancePayment()
        {
        }

        public ResultFRBODocumentFinancePayment(FRBOGenericCollection<FRBODocumentFinancePaymentView> pFRBODocumentFinancePayment)
        {
            _documentFinancePayment = pFRBODocumentFinancePayment;
        }
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

    public class FRBOHelper
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Generate Fast Report Business Objects for ProcessReportFinanceDocument
        /// </summary>
        public static ResultFRBODocumentFinanceMaster GetFRBOFinanceDocument(Guid pDocumentFinanceMasterOid)
        {
            ResultFRBODocumentFinanceMaster result = new ResultFRBODocumentFinanceMaster();

            try
            {
                FIN_DocumentFinanceMaster documentFinanceMaster = (FIN_DocumentFinanceMaster)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(FIN_DocumentFinanceMaster), pDocumentFinanceMasterOid);

                bool retificationDocuments = (
                     documentFinanceMaster.DocumentType.Oid == SettingsApp.XpoOidDocumentFinanceTypeCreditNote
                );

                string sqlFilter = string.Format("fmOid = '{0}'", documentFinanceMaster.Oid.ToString());

                //Prepare and Declare FRBOGenericCollections : Limit 1, One Record (Document Master), else we get All Details to (View)
                FRBOGenericCollection<FRBODocumentFinanceMasterView> gcDocumentFinanceMaster = new FRBOGenericCollection<FRBODocumentFinanceMasterView>(sqlFilter, 1);
                FRBOGenericCollection<FRBODocumentFinanceDetail> gcDocumentFinanceDetail;
                FRBOGenericCollection<FRBODocumentFinanceMasterTotalView> gcDocumentFinanceMasterTotal;

                //Override Default Values

                //If Simplified Invoice - Remove Customer Details (If System Country Equal to PT)
                if (
                    (
                        SettingsApp.XpoOidConfigurationCountryPortugal == SettingsApp.ConfigurationSystemCountry.Oid &&
                        new Guid(gcDocumentFinanceMaster.List[0].DocumentType) == SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice
                    )
                    //Added
                    || FrameworkUtils.GetFinalConsumerEntity().Oid.ToString() == gcDocumentFinanceMaster.List[0].EntityOid
                    //Removed This Way we only clean Entity data if is a SimplifiedInvoice, to protect Hidden Entitys with FinanceFinalConsumerFiscalNumber 
                    //|| (
                    //    gcDocumentFinanceMaster.List[0].EntityFiscalNumber == SettingsApp.FinanceFinalConsumerFiscalNumber 
                    //)
                )
                {
                    gcDocumentFinanceMaster.List[0].EntityName = string.Empty;
                    gcDocumentFinanceMaster.List[0].EntityAddress = string.Empty;
                    gcDocumentFinanceMaster.List[0].EntityZipCode = string.Empty;
                    gcDocumentFinanceMaster.List[0].EntityCity = string.Empty;
                    gcDocumentFinanceMaster.List[0].EntityCountry = string.Empty;
                }

                //Detect if is FinalConsumer with FiscalNumber 999999990 and Hide it
                ERP_Customer customer = (ERP_Customer)GlobalFramework.SessionXpo.GetObjectByKey(typeof(ERP_Customer), SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity);
                if (gcDocumentFinanceMaster.List[0].EntityFiscalNumber == customer.FiscalNumber)
                {
                    gcDocumentFinanceMaster.List[0].EntityFiscalNumber = SettingsApp.FinanceFinalConsumerFiscalNumberDisplay;
                }

                //Detect if is Retification Document (ND/NC) and add SourceDocument to Show on Notes
                if (! string.IsNullOrEmpty(gcDocumentFinanceMaster.List[0].ATDocCodeID))
                {
                    string notes = string.Format("{0} : {1}", Resx.global_at_atdoccodeid, gcDocumentFinanceMaster.List[0].ATDocCodeID);
                    if (! string.IsNullOrEmpty(gcDocumentFinanceMaster.List[0].Notes)) notes += " | "/*Environment.NewLine*/;
                    notes += gcDocumentFinanceMaster.List[0].Notes;
                    gcDocumentFinanceMaster.List[0].Notes = notes;
                }

                //Detect if is Retification Document (ND/NC) and add SourceDocument to Show on Notes
                if (retificationDocuments)
                {
                    string notes = string.Format("{0} : {1}", Resx.global_source_document, documentFinanceMaster.DocumentParent.DocumentNumber);
                    if (! string.IsNullOrEmpty(gcDocumentFinanceMaster.List[0].Notes)) notes += " | "/*Environment.NewLine*/;
                    notes += gcDocumentFinanceMaster.List[0].Notes;
                    gcDocumentFinanceMaster.List[0].Notes = notes;
                }

                //Render Child Bussiness Objects
                foreach (FRBODocumentFinanceMasterView documentMaster in gcDocumentFinanceMaster)
                {
                    //Get FinanceDetail 
                    gcDocumentFinanceDetail = new FRBOGenericCollection<FRBODocumentFinanceDetail>(string.Format("DocumentMaster = '{0}'", documentMaster.Oid), "Ord");
                    documentMaster.DocumentFinanceDetail = gcDocumentFinanceDetail.List;

                    //Get FinanceMasterTotals
                    gcDocumentFinanceMasterTotal = new FRBOGenericCollection<FRBODocumentFinanceMasterTotalView>(string.Format("fmtDocumentMaster = '{0}'", documentMaster.Oid), "Value");
                    documentMaster.DocumentFinanceMasterTotal = gcDocumentFinanceMasterTotal.List;
                }

                result.DocumentFinanceMaster = gcDocumentFinanceMaster;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        /// <summary>
        /// Generate Fast Report Business Objects for ProcessReportFinanceDocumentPayment
        /// </summary>
        public static ResultFRBODocumentFinancePayment GetFRBOFinancePayment(Guid pDocumentFinancePaymentOid)
        {
            ResultFRBODocumentFinancePayment result = new ResultFRBODocumentFinancePayment();

            try
            {
                FIN_DocumentFinancePayment documentFinancePayment = (FIN_DocumentFinancePayment)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(FIN_DocumentFinancePayment), pDocumentFinancePaymentOid);

                string sqlFilter = string.Format("fpaOid = '{0}'", pDocumentFinancePaymentOid.ToString());

                //Prepare and Declare FRBOGenericCollections
                FRBOGenericCollection<FRBODocumentFinancePaymentView> gcDocumentFinancePayment = new FRBOGenericCollection<FRBODocumentFinancePaymentView>(sqlFilter);
                FRBOGenericCollection<FRBODocumentFinancePaymentDocumentView> gcDocumentFinancePaymentDocument;

                //Decrypt Values
                gcDocumentFinancePayment.List[0].EntityName = XPGuidObject.DecryptIfNeeded(gcDocumentFinancePayment.List[0].EntityName).ToString();
                gcDocumentFinancePayment.List[0].EntityAddress = XPGuidObject.DecryptIfNeeded(gcDocumentFinancePayment.List[0].EntityAddress).ToString();
                gcDocumentFinancePayment.List[0].EntityZipCode = XPGuidObject.DecryptIfNeeded(gcDocumentFinancePayment.List[0].EntityZipCode).ToString();
                gcDocumentFinancePayment.List[0].EntityCity = XPGuidObject.DecryptIfNeeded(gcDocumentFinancePayment.List[0].EntityCity).ToString();
                gcDocumentFinancePayment.List[0].EntityFiscalNumber = XPGuidObject.DecryptIfNeeded(gcDocumentFinancePayment.List[0].EntityFiscalNumber).ToString();

                //If FinalConsumer - Clean Output Data
                if (gcDocumentFinancePayment.List[0].EntityFiscalNumber == SettingsApp.FinanceFinalConsumerFiscalNumber)
                {
                    gcDocumentFinancePayment.List[0].EntityName = string.Empty;
                    gcDocumentFinancePayment.List[0].EntityAddress = string.Empty;
                    gcDocumentFinancePayment.List[0].EntityZipCode = string.Empty;
                    gcDocumentFinancePayment.List[0].EntityCity = string.Empty;
                    gcDocumentFinancePayment.List[0].EntityFiscalNumber = SettingsApp.FinanceFinalConsumerFiscalNumberDisplay;
                }

                //Render Child Bussiness Objects
                foreach (FRBODocumentFinancePaymentView documentFinanceMasterPayment in gcDocumentFinancePayment)
                {
                    //Get FinanceDetail 
                    gcDocumentFinancePaymentDocument = new FRBOGenericCollection<FRBODocumentFinancePaymentDocumentView>(string.Format("fpaOid = '{0}'", documentFinanceMasterPayment.Oid), "ftpCode, fmaDocumentNumber");
                    documentFinanceMasterPayment.DocumentFinancePaymentDocument = gcDocumentFinancePaymentDocument.List;
                }

                result.DocumentFinancePayment = gcDocumentFinancePayment;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }
    }
}

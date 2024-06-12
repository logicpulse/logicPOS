using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Reporting.Data.Common;
using LogicPOS.Reporting.Reports.Data;
using LogicPOS.Settings;
using LogicPOS.Shared.CustomDocument;
using System;
using System.Reflection;

namespace LogicPOS.Reporting.Common
{
    public class ReportHelper
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Generate Fast Report Business Objects for ProcessReportFinanceDocument
        /// </summary>
        public static ReportList<FinanceMasterViewReportData> GetFinanceMasterViewReports(Guid financeMasterId)
        {
            ReportList<FinanceMasterViewReportData> result = new ReportList<FinanceMasterViewReportData>();

            try
            {
                fin_documentfinancemaster documentFinanceMaster = XPOUtility.GetEntityById<fin_documentfinancemaster>(financeMasterId);

                bool retificationDocuments =
                     documentFinanceMaster.DocumentType.Oid == CustomDocumentSettings.CreditNoteId
                ;
                /* IN009173 */
                bool isTransportDocument =
                    documentFinanceMaster.DocumentType.Oid == CustomDocumentSettings.TransportDocumentTypeId ||
                    documentFinanceMaster.DocumentType.Oid == CustomDocumentSettings.DeliveryNoteDocumentTypeId
                ;

                string sqlFilter = string.Format("fmOid = '{0}'", documentFinanceMaster.Oid.ToString());

                //Prepare and Declare FRBOGenericCollections : Limit 1, One Record (Document Master), else we get All Details to (View)
                ReportList<FinanceMasterViewReportData> gcDocumentFinanceMaster = new ReportList<FinanceMasterViewReportData>(sqlFilter, 1);
                ReportList<FinanceDetailReportData> gcDocumentFinanceDetail;
                ReportList<FinanceMasterTotalViewReportData> gcDocumentFinanceMasterTotal;
                /* IN005986 - code refactoring */
                FinanceMasterViewReportData documentFinanceMasterView = gcDocumentFinanceMaster.List[0];

                /* IN009075 - for decrypt phase */
                bool customerDataHasBeenCleaned = false;

                //Override Default Values
                //If Simplified Invoice - Remove Customer Details (If System Country Equal to PT)
                if (XPOSettings.ConfigurationSystemCountry.Oid.Equals(CultureSettings.PortugalCountryId)
                    || XPOSettings.ConfigurationSystemCountry.Oid.Equals(CultureSettings.MozambiqueCountryId) /* IN005986 */
                    || XPOSettings.ConfigurationSystemCountry.Oid.Equals(CultureSettings.AngolaCountryId)) /* IN009230 - Angola is now added to this rule */
                {
                    /* IN009230 - now, only when Final Customer we have data cleaned */
                    //if (SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice.Equals(new Guid(documentFinanceMasterView.DocumentType)) 
                    //    || FrameworkUtils.GetFinalConsumerEntity().Oid.ToString() == documentFinanceMasterView.EntityOid) //Added
                    if (XPOUtility.GetFinalConsumerEntity().Oid.ToString() == documentFinanceMasterView.EntityOid) //Added
                    {
                        documentFinanceMasterView.EntityName = string.Empty;
                        documentFinanceMasterView.EntityAddress = string.Empty;
                        documentFinanceMasterView.EntityZipCode = string.Empty;
                        documentFinanceMasterView.EntityCity = string.Empty;
                        documentFinanceMasterView.EntityCountry = string.Empty;
                        documentFinanceMasterView.EntityLocality = string.Empty;
                        /* IN009230 */
                        documentFinanceMasterView.EntityFiscalNumber = SaftSettings.FinanceFinalConsumerFiscalNumberDisplay;

                        customerDataHasBeenCleaned = true;
                    }
                    /* IN009230 - "If" content removed from here to the just before block of code */
                    //Detect if is FinalConsumer with FiscalNumber 999999990 and Hide it
                    //erp_customer customer = (erp_customer)XPOSettings.Session.GetObjectByKey(typeof(erp_customer), SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity);
                    //if (documentFinanceMasterView.EntityFiscalNumber == customer.FiscalNumber)
                    //{
                    //    documentFinanceMasterView.EntityFiscalNumber = SettingsApp.FinanceFinalConsumerFiscalNumberDisplay;
                    //}
                }
                //IN009347 Documentos PT - Alteração do Layout dos dados do Cliente #Lindote 2020
                /* IN009075 - decrypt phase */
                if (!customerDataHasBeenCleaned)
                {
                    documentFinanceMasterView.EntityName = PluginSettings.SoftwareVendor.Decrypt(documentFinanceMasterView.EntityName);
                    documentFinanceMasterView.EntityAddress = PluginSettings.SoftwareVendor.Decrypt(documentFinanceMasterView.EntityAddress);
                    documentFinanceMasterView.EntityZipCode = PluginSettings.SoftwareVendor.Decrypt(documentFinanceMasterView.EntityZipCode);
                    documentFinanceMasterView.EntityCity = PluginSettings.SoftwareVendor.Decrypt(documentFinanceMasterView.EntityCity);
                    documentFinanceMasterView.EntityLocality = PluginSettings.SoftwareVendor.Decrypt(documentFinanceMasterView.EntityLocality);
                    // documentFinanceMasterView.EntityCountry = LogicPOS.Settings.PluginSettings.PluginSoftwareVendor.Decrypt(documentFinanceMasterView.EntityCountry);
                    // EntityLocality???
                    /* IN009230 */
                    documentFinanceMasterView.EntityFiscalNumber = PluginSettings.SoftwareVendor.Decrypt(documentFinanceMasterView.EntityFiscalNumber);
                }

                /* IN009173 - add Parent document number to Notes field */
                if (isTransportDocument && documentFinanceMaster.DocumentParent != null)
                {
                    string notes = string.Format("{0}: {1}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_source_document"), documentFinanceMaster.DocumentParent.DocumentNumber);
                    if (!string.IsNullOrEmpty(documentFinanceMasterView.Notes)) notes += " | ";
                    notes += documentFinanceMasterView.Notes;
                    documentFinanceMasterView.Notes = notes;
                }
                //ATCUD Documentos - Criação do QRCode e ATCUD IN016508
                documentFinanceMasterView.ATDocQRCode = documentFinanceMaster.ATDocQRCode;
                /* Add ATDocCodeID to Notes field */
                if (!string.IsNullOrEmpty(documentFinanceMasterView.ATDocCodeID))
                {
                    string notes = string.Format("{0}: {1}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_at_atdoccodeid"), documentFinanceMasterView.ATDocCodeID);
                    if (!string.IsNullOrEmpty(documentFinanceMasterView.Notes)) notes += " | "/*Environment.NewLine*/;
                    notes += documentFinanceMasterView.Notes;
                    documentFinanceMasterView.Notes = notes;
                }

                //Detect if is Retification Document (ND/NC) and add SourceDocument to Show on Notes
                if (retificationDocuments)
                {
                    //TK016319 - Certificação Angola - Alterações para teste da AGT
                    //Notas de Credito
                    string notes = string.Format("{0}: {1}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_source_document"), documentFinanceMaster.DocumentParent.DocumentNumber);
                    if (CultureSettings.AngolaCountryId.Equals(XPOSettings.ConfigurationSystemCountry.Oid))
                    {
                        notes = string.Format("{0}: {1}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_source_document_NC_ND"), documentFinanceMaster.DocumentParent.DocumentNumber);
                    }
                    /* IN009252 - "Reason" added to "fin_documentfinancemaster.Notes" */
                    if (!string.IsNullOrEmpty(documentFinanceMasterView.Notes)) notes += Environment.NewLine; /* " | " */
                    notes += string.Format("{0}: {1}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_reason"), documentFinanceMasterView.Notes);
                    documentFinanceMasterView.Notes = notes;
                }

                //Render Child Bussiness Objects
                foreach (FinanceMasterViewReportData documentMaster in gcDocumentFinanceMaster)
                {
                    //Get FinanceDetail 
                    gcDocumentFinanceDetail = new ReportList<FinanceDetailReportData>(string.Format("DocumentMaster = '{0}'", documentMaster.Oid), "Ord");
                    documentMaster.DocumentFinanceDetail = gcDocumentFinanceDetail.List;

                    //Get FinanceMasterTotals
                    gcDocumentFinanceMasterTotal = new ReportList<FinanceMasterTotalViewReportData>(string.Format("fmtDocumentMaster = '{0}'", documentMaster.Oid), "Value");
                    documentMaster.DocumentFinanceMasterTotal = gcDocumentFinanceMasterTotal.List;
                }

                result = gcDocumentFinanceMaster;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        public static ReportList<FinancePaymentViewReportData> GetFinancePaymentViewReports(Guid financePaymentId)
        {
            ReportList<FinancePaymentViewReportData> result = new ReportList<FinancePaymentViewReportData>();

            try
            {
                fin_documentfinancepayment documentFinancePayment = XPOUtility.GetEntityById<fin_documentfinancepayment>(financePaymentId);

                string sqlFilter = string.Format("fpaOid = '{0}'", financePaymentId.ToString());

                //Prepare and Declare FRBOGenericCollections
                ReportList<FinancePaymentViewReportData> gcDocumentFinancePayment = new ReportList<FinancePaymentViewReportData>(sqlFilter);
                ReportList<FinancePaymentDocumentViewReportData> gcDocumentFinancePaymentDocument;

                //Decrypt Values
                if (!string.IsNullOrEmpty(gcDocumentFinancePayment.List[0].EntityName))
                    gcDocumentFinancePayment.List[0].EntityName = Entity.DecryptIfNeeded(gcDocumentFinancePayment.List[0].EntityName).ToString();
                if (!string.IsNullOrEmpty(gcDocumentFinancePayment.List[0].EntityAddress))
                    gcDocumentFinancePayment.List[0].EntityAddress = Entity.DecryptIfNeeded(gcDocumentFinancePayment.List[0].EntityAddress).ToString();
                if (!string.IsNullOrEmpty(gcDocumentFinancePayment.List[0].EntityZipCode))
                    gcDocumentFinancePayment.List[0].EntityZipCode = Entity.DecryptIfNeeded(gcDocumentFinancePayment.List[0].EntityZipCode).ToString();
                if (!string.IsNullOrEmpty(gcDocumentFinancePayment.List[0].EntityCity))
                    gcDocumentFinancePayment.List[0].EntityCity = Entity.DecryptIfNeeded(gcDocumentFinancePayment.List[0].EntityCity).ToString();
                if (!string.IsNullOrEmpty(gcDocumentFinancePayment.List[0].EntityLocality))
                    gcDocumentFinancePayment.List[0].EntityLocality = Entity.DecryptIfNeeded(gcDocumentFinancePayment.List[0].EntityLocality).ToString();
                if (!string.IsNullOrEmpty(gcDocumentFinancePayment.List[0].EntityFiscalNumber))
                    gcDocumentFinancePayment.List[0].EntityFiscalNumber = Entity.DecryptIfNeeded(gcDocumentFinancePayment.List[0].EntityFiscalNumber).ToString();

                //If FinalConsumer - Clean Output Data
                if (gcDocumentFinancePayment.List[0].EntityFiscalNumber == SaftSettings.FinanceFinalConsumerFiscalNumber)
                {
                    gcDocumentFinancePayment.List[0].EntityName = string.Empty;
                    gcDocumentFinancePayment.List[0].EntityAddress = string.Empty;
                    gcDocumentFinancePayment.List[0].EntityZipCode = string.Empty;
                    gcDocumentFinancePayment.List[0].EntityCity = string.Empty;
                    gcDocumentFinancePayment.List[0].EntityLocality = string.Empty;
                    gcDocumentFinancePayment.List[0].EntityFiscalNumber = SaftSettings.FinanceFinalConsumerFiscalNumberDisplay;
                }

                //Render Child Bussiness Objects
                foreach (FinancePaymentViewReportData documentFinanceMasterPayment in gcDocumentFinancePayment)
                {
                    //Get FinanceDetail 
                    gcDocumentFinancePaymentDocument = new ReportList<FinancePaymentDocumentViewReportData>(string.Format("fpaOid = '{0}'", documentFinanceMasterPayment.Oid), "ftpCode, fmaDocumentNumber");
                    documentFinanceMasterPayment.DocumentFinancePaymentDocument = gcDocumentFinancePaymentDocument.List;
                }

                result = gcDocumentFinancePayment;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }
    }
}

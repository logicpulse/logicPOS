using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.console.App;
using logicpos.financial.library.Classes.Finance;
using logicpos.financial.library.Classes.Hardware.Printers;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Enums;
using System;
using System.Collections.Generic;

namespace logicpos.financial.console.Test.Classes
{
    public class TestProcessFinanceDocument
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceDocument

        //Used to trigger all Errors
        public static FIN_DocumentFinanceMaster PersistFinanceDocumentMinimal(Guid pDocumentFinanceType)
        {
            //Store current Logged Details
            SYS_UserDetail loggedUser = GlobalFramework.LoggedUser;
            POS_ConfigurationPlaceTerminal loggedTerminal = GlobalFramework.LoggedTerminal;

            //Reset Current Logged Details
            GlobalFramework.LoggedUser = null;
            GlobalFramework.LoggedTerminal = null;
            FIN_DocumentFinanceMaster documentFinanceMaster = null;

            try
            {
                //Create Empty ArticleBag
                ArticleBag articleBag = new ArticleBag();
                //Create ProcessFinanceDocumentParameter
                ProcessFinanceDocumentParameter processFinanceDocumentParameter = new ProcessFinanceDocumentParameter(pDocumentFinanceType, articleBag);
                //Reset Defaults
                processFinanceDocumentParameter.Currency = Guid.Empty;
                documentFinanceMaster = ProcessFinanceDocument.PersistFinanceDocument(processFinanceDocumentParameter);
                Console.WriteLine(string.Format("documentFinanceMaster.DocumentNumber: [{0}]", documentFinanceMaster.DocumentNumber));
            }
            finally
            {
                //Restore Old Logged Details
                GlobalFramework.LoggedUser = loggedUser;
                GlobalFramework.LoggedTerminal = loggedTerminal;
            }

            return documentFinanceMaster;
        }

        public static FIN_DocumentFinanceMaster PersistFinanceDocumentBase(Guid pDocumentFinanceType, ProcessFinanceDocumentParameter pProcessFinanceDocumentParameter)
        {
            ArticleBag articleBag = TestArticleBag.GetArticleBag(false);

            //Change default DocumentDateTime
            //processFinanceDocumentParameter.DocumentDateTime = FrameworkUtils.CurrentDateTimeAtomic().AddDays(-5);

            FIN_DocumentFinanceMaster documentFinanceMaster = ProcessFinanceDocument.PersistFinanceDocument(pProcessFinanceDocumentParameter);
            if (documentFinanceMaster != null)
            {
                Console.WriteLine(string.Format("documentFinanceMaster.DocumentNumber: [{0}]", documentFinanceMaster.DocumentNumber));
                PrintRouter.PrintFinanceDocument(documentFinanceMaster);
            }

            return documentFinanceMaster;
        }

        //Default
        public static FIN_DocumentFinanceMaster PersistFinanceDocument(Guid pDocumentFinanceType)
        {
            ArticleBag articleBag = TestArticleBag.GetArticleBag(false);

            ProcessFinanceDocumentParameter processFinanceDocumentParameter = new ProcessFinanceDocumentParameter(pDocumentFinanceType, articleBag) {
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag,
                //P1
                PaymentCondition = SettingsApp.XpoOidDocumentPaymentCondition,
                PaymentMethod = SettingsApp.XpoOidDocumentPaymentMethod,
                Currency = SettingsApp.XpoOidDocumentCurrency,
                //P2
                Customer = SettingsApp.XpoOidDocumentCustomer

            };

            return PersistFinanceDocumentBase(pDocumentFinanceType, processFinanceDocumentParameter);
        }

        //Credit Notes
        public static FIN_DocumentFinanceMaster PersistFinanceDocumentCreditNote(Guid pDocumentFinanceType)
        {
            //SourceDocument for CreditNote
            Guid xpoOidParentDocument = new Guid("316528f6-bf9b-4a6d-aa5b-530379aaa6ef");
            FIN_DocumentFinanceMaster sourceDocument = (FIN_DocumentFinanceMaster) GlobalFramework.SessionXpo.GetObjectByKey(typeof(FIN_DocumentFinanceMaster), xpoOidParentDocument);

            ArticleBag articleBag = TestArticleBag.GetArticleBag(false);

            //Fill Required Reference and Reason 
            foreach (var item in articleBag)
            {
                item.Value.Reference = sourceDocument;
                item.Value.Reason = "Anulação";
                item.Value.Quantity -= 1;
            }

            ProcessFinanceDocumentParameter processFinanceDocumentParameter = new ProcessFinanceDocumentParameter(pDocumentFinanceType, articleBag) {
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag,
                //P1
                PaymentCondition = SettingsApp.XpoOidDocumentPaymentCondition,
                PaymentMethod = SettingsApp.XpoOidDocumentPaymentMethod,
                Currency = SettingsApp.XpoOidDocumentCurrency,
                //Used for Credit Notes
                DocumentParent = xpoOidParentDocument,
                //P2
                Customer = SettingsApp.XpoOidDocumentCustomer

            };

            return PersistFinanceDocumentBase(pDocumentFinanceType, processFinanceDocumentParameter);
        }         

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceDocumentPayments

        public static FIN_DocumentFinancePayment PersistFinanceDocumentPayment()
        {
            //All Documents, usefull to Split into Invoices and CreditNotes
            List<Guid> listDocuments = new List<Guid>();
            List<FIN_DocumentFinanceMaster> listInvoices = new List<FIN_DocumentFinanceMaster>();
            List<FIN_DocumentFinanceMaster> listCreditNotes = new List<FIN_DocumentFinanceMaster>();
            //listDocuments.Add(new Guid("a067b55b-09b5-4a48-a386-e9d04777540b"));//NC NCCKOQ5R5012016S001/1 
            //listDocuments.Add(new Guid("92c5c809-0387-4d1f-93c6-afeb6f291149"));//FT FTCKOQ5R5012016S001/1
            //Used
            listDocuments.Add(new Guid("e00cde4c-2218-4c03-ac4b-161b2b89032b"));//NC NCCKOQ5R5012016S001/2
            listDocuments.Add(new Guid("ed8e5198-f253-4198-be18-fff188a7d3b4"));//NC NCCKOQ5R5012016S001/3
            listDocuments.Add(new Guid("dc4b8170-2b61-4ed4-ad8b-77aaf8a4c695"));//FT FTCKOQ5R5012016S001/3
            listDocuments.Add(new Guid("36e49eb1-5e94-47e4-9467-8365ec71e099"));//FT FTCKOQ5R5012016S001/4
            listDocuments.Add(new Guid("529f5d2d-4202-4207-b15f-2d58d56c4ebe"));//FT FTCKOQ5R5012016S001/6

            //Force Errors : Invalid Documents
            listDocuments.Add(new Guid("41ba3dfa-8afa-48bd-aa56-1b51015e7c0c"));//GT GTCKOQ5R5012016S001/1 Mário Fernandes invalid document + diferent customer 
            listDocuments.Add(new Guid("a067b55b-09b5-4a48-a386-e9d04777540b"));//NC NCCKOQ5R5012016S001/1 Carlos Fernandes Payed
            listDocuments.Add(new Guid("92c5c809-0387-4d1f-93c6-afeb6f291149"));//FT FTCKOQ5R5012016S001/1 Carlos Fernandes Payed
            listDocuments.Add(new Guid("2813d769-3065-44fb-b48f-a8f7a42a0456"));//FT FTCKOQ5R5012016S001/5 Carlos DocumentStatusStatus = A

            //Other Parameters
            //decimal paymentAmount = 28.17m;//Euro
            decimal paymentAmount = 517.06m;//Kwanza
            string paymentNotes = "$#%#$&%»»'@£§€";

            //Prepare listInvoices and listCreditNotes
            foreach (Guid item in listDocuments)
            {
                FIN_DocumentFinanceMaster documentFinanceMaster = (FIN_DocumentFinanceMaster)GlobalFramework.SessionXpo.GetObjectByKey(typeof(FIN_DocumentFinanceMaster), item);

                if (documentFinanceMaster.DocumentType.Credit)
                {
                    listInvoices.Add(documentFinanceMaster);
                }
                else
                {
                    listCreditNotes.Add(documentFinanceMaster);
                }
            }

            FIN_DocumentFinancePayment documentFinancePayment = ProcessFinanceDocument.PersistFinanceDocumentPayment(listInvoices, listCreditNotes, SettingsApp.XpoOidDocumentCustomer, SettingsApp.XpoOidDocumentPaymentMethod, SettingsApp.XpoOidDocumentCurrency, paymentAmount, paymentNotes);

            if (documentFinancePayment != null)
            {
                Console.WriteLine(string.Format("documentFinancePayment.PaymentRefNo: [{0}]", documentFinancePayment.PaymentRefNo));
                SYS_ConfigurationPrintersTemplates template = ProcessFinanceDocumentSeries.GetDocumentFinanceYearSerieTerminal(GlobalFramework.SessionXpo, documentFinancePayment.DocumentType.Oid).Template;
                PrintRouter.PrintFinanceDocumentPayment(GlobalFramework.LoggedTerminal.Printer, documentFinancePayment);
            }

            return documentFinancePayment;
        }
    }
}

using Gtk;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.library.Classes.Finance;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Classes.Orders;
using logicpos.shared.Enums;
using System;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog
{
    /// <summary>
    /// Test Page : Not Used
    /// </summary>

    class DocumentFinanceDialogPage7 : PagePadPage
    {
        private VBox _vboxButtons;
        private HBox _hboxButtons1;
        private HBox _hboxButtons2;
        private OrderMain _orderMain;
        private ArticleBag _articleBag;

        //Constructor
        public DocumentFinanceDialogPage7(Window pSourceWindow, String pPageName) : this(pSourceWindow, pPageName, "", null, true) { }
        public DocumentFinanceDialogPage7(Window pSourceWindow, String pPageName, Widget pWidget) : this(pSourceWindow, pPageName, "", pWidget, true) { }
        public DocumentFinanceDialogPage7(Window pSourceWindow, String pPageName, String pPageIcon, Widget pWidget, bool pEnabled = true)
            : base(pSourceWindow, pPageName, pPageIcon, pWidget, pEnabled)
        {
            _vboxButtons = new VBox(true, 0);
            _hboxButtons1 = new HBox(true, 0);
            _hboxButtons2 = new HBox(true, 0);

            //Print Invoice
            Button buttonPrintInvoice = new Button("Print Invoice") { Sensitive = false };
            buttonPrintInvoice.Clicked += buttonPrintInvoice_Clicked;

            //Cancel Invoice
            Button buttonCancelInvoice = new Button("Cancel Invoice");
            buttonCancelInvoice.Clicked += buttonCancelInvoice_Clicked;

            //OrderReferences
            Button buttonOrderReferences = new Button("Order References");
            buttonOrderReferences.Clicked += buttonOrderReferences_Clicked;

            //Credit Note
            Button buttonCreditNote = new Button("Credit Note");
            buttonCreditNote.Clicked += buttonCreditNote_Clicked;

            //Print Invoice With Diferent Vat's
            Button buttonPrintInvoiceVat = new Button("Print Invoice Vat");
            buttonPrintInvoiceVat.Clicked += buttonPrintInvoiceVat_Clicked;

            //Print Invoice With Discounts
            Button buttonPrintInvoiceDiscount = new Button("Print Invoice Discount");
            buttonPrintInvoiceDiscount.Clicked += buttonPrintInvoiceDiscount_Clicked;

            //Print Invoice With ExchangeRate
            Button buttonPrintInvoiceExchangeRate = new Button("Print Invoice ExchangeRate");
            buttonPrintInvoiceExchangeRate.Clicked += buttonPrintInvoiceExchangeRate_Clicked;

            //Print Invoice With JohnDoe1
            Button buttonPrintInvoiceJohnDoe1 = new Button("Print Invoice JohnDoe1");
            buttonPrintInvoiceJohnDoe1.Clicked += buttonPrintInvoiceJohnDoe1_Clicked;

            //Print Invoice With JohnDoe2
            Button buttonPrintInvoiceJohnDoe2 = new Button("Print Invoice JohnDoe2");
            buttonPrintInvoiceJohnDoe2.Clicked += buttonPrintInvoiceJohnDoe2_Clicked;

            //Print Invoice Transportation Guide With Totals
            Button buttonPrintTransportationGuideWithTotals = new Button("Print Transportation Guide With Totals");
            buttonPrintTransportationGuideWithTotals.Clicked += buttonPrintTransportationGuideWithTotals_Clicked;

            //Print Invoice Transportation Guide Without Totals
            Button buttonPrintTransportationGuideWithoutTotals = new Button("Print Transportation Guide Without Totals");
            buttonPrintTransportationGuideWithoutTotals.Clicked += buttonPrintTransportationGuideWithoutTotals_Clicked;

            //Pack hboxButtons1
            _hboxButtons1.PackStart(buttonPrintInvoice, true, true, 2);
            _hboxButtons1.PackStart(buttonCancelInvoice, true, true, 2);
            _hboxButtons1.PackStart(buttonOrderReferences, true, true, 2);
            _hboxButtons1.PackStart(buttonCreditNote, true, true, 2);
            _hboxButtons1.PackStart(buttonPrintInvoiceVat, true, true, 2);
            _hboxButtons1.PackStart(buttonPrintInvoiceDiscount, true, true, 2);
            //Pack hboxButtons2
            _hboxButtons2.PackStart(buttonPrintInvoiceExchangeRate, true, true, 2);
            _hboxButtons2.PackStart(buttonPrintInvoiceJohnDoe1, true, true, 2);
            _hboxButtons2.PackStart(buttonPrintInvoiceJohnDoe2, true, true, 2);
            _hboxButtons2.PackStart(buttonPrintTransportationGuideWithTotals, true, true, 2);
            _hboxButtons2.PackStart(buttonPrintTransportationGuideWithoutTotals, true, true, 2);

            //Shared : Prepare ArticleBag
            if (GlobalFramework.SessionApp.OrdersMain.ContainsKey(GlobalFramework.SessionApp.CurrentOrderMainOid))
            {
                _orderMain = GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid];
                if (_orderMain != null) _articleBag = ArticleBag.TicketOrderToArticleBag(_orderMain);
                if (_articleBag != null && _articleBag.Count > 0)
                {
                    buttonPrintInvoice.Sensitive = true;
                }
            }

            _vboxButtons.PackStart(_hboxButtons1, true, true, 2);
            _vboxButtons.PackStart(_hboxButtons2, true, true, 2);

            PackStart(_vboxButtons);
        }

        //Override Base Validate
        public override void Validate()
        {
            //_log.Debug(string.Format("Validate: {0}", this.Name));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        //5.2: FT: Fatura
        void buttonPrintInvoice_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = SettingsApp.XpoOidDocumentFinanceTypeInvoice;
            Guid customerGuid = new Guid("6223881a-4d2d-4de4-b254-f8529193da33");

            //Prepare ProcessFinanceDocumentParameter
            ProcessFinanceDocumentParameter processFinanceDocumentParameter = new ProcessFinanceDocumentParameter(
              documentTypeGuid, _articleBag)
            {
                Customer = customerGuid
            };

            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
            _vboxButtons.Sensitive = false;
        }

        //5.3: FT: Cancel Invoice
        void buttonCancelInvoice_Clicked(object sender, EventArgs e)
        {
            string dateTimeFormatCombinedDateTime = SettingsApp.DateTimeFormatCombinedDateTime;
            Guid documentMasterGuid = new Guid("81fcf207-ff59-4971-90cb-80d2cbdb87dc");//Document To Cancel
            fin_documentfinancemaster documentFinanceMaster = (fin_documentfinancemaster)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_documentfinancemaster), documentMasterGuid);

            //Cancel Document
            documentFinanceMaster.DocumentStatusStatus = "A";
            documentFinanceMaster.DocumentStatusDate = FrameworkUtils.CurrentDateTimeAtomic().ToString(dateTimeFormatCombinedDateTime);
            documentFinanceMaster.DocumentStatusReason = "Erro ao Inserir Artigos";
            documentFinanceMaster.Save();
        }

        //OrderReferences
        void buttonOrderReferences_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = SettingsApp.XpoOidDocumentFinanceTypeInvoice;
            Guid customerGuid = new Guid("6223881a-4d2d-4de4-b254-f8529193da33");
            Guid orderReference = new Guid("fbec0056-71a7-4d5b-8bfa-d5e887ec585f");

            //DC DC2015S0001/1
            fin_documentfinancemaster documentOrderReference = (fin_documentfinancemaster)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_documentfinancemaster), orderReference);
            //Add Order References
            List<fin_documentfinancemaster> orderReferences = new List<fin_documentfinancemaster>();
            orderReferences.Add(documentOrderReference);

            //Get ArticleBag from documentFinanceMasterSource
            ArticleBag articleBag = ArticleBag.DocumentFinanceMasterToArticleBag(documentOrderReference);

            //Prepare ProcessFinanceDocumentParameter
            ProcessFinanceDocumentParameter processFinanceDocumentParameter = new ProcessFinanceDocumentParameter(
              documentTypeGuid, articleBag)
            {
                Customer = customerGuid,
                OrderReferences = orderReferences,
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag,
                SourceOrderMain = documentOrderReference.SourceOrderMain
            };

            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
        }

        //NC : Credit Note
        void buttonCreditNote_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = SettingsApp.XpoOidDocumentFinanceTypeCreditNote;
            Guid reference = new Guid("daecbf1d-6211-4e74-a8cd-81795e347656");

            //FT FT2015S0001/16
            fin_documentfinancemaster documentReference = (fin_documentfinancemaster)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_documentfinancemaster), reference);
            //Add Order References
            List<DocumentReference> references = new List<DocumentReference>();
            references.Add(new DocumentReference(documentReference, "Artigo com defeito"));

            //Get ArticleBag from documentFinanceMasterSource
            ArticleBag articleBag = ArticleBag.DocumentFinanceMasterToArticleBag(documentReference);

            //Prepare ProcessFinanceDocumentParameter
            ProcessFinanceDocumentParameter processFinanceDocumentParameter = new ProcessFinanceDocumentParameter(
              documentTypeGuid, articleBag)
            {
                Customer = documentReference.EntityOid,
                //References = references,
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag
            };
            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
        }

        //FT: Vats
        void buttonPrintInvoiceVat_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = SettingsApp.XpoOidDocumentFinanceTypeInvoice;
            Guid customerGuid = new Guid("6223881a-4d2d-4de4-b254-f8529193da33");
            Guid vatExemptionReasonGuid = new Guid("8311ce58-50ee-4115-9cf9-dbca86538fdd");
            fin_configurationvatexemptionreason vatExemptionReason = (fin_configurationvatexemptionreason)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_configurationvatexemptionreason), vatExemptionReasonGuid);

            //Article:Line1
            Guid articleREDGuid = new Guid("72e8bde8-d03b-4637-90f1-fcb265658af0");
            fin_article articleRED = (fin_article)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_article), articleREDGuid);
            //Article:Line2
            Guid articleISEGuid = new Guid("78638720-e728-4e96-8643-6d6267ff817b");
            fin_article articleISE = (fin_article)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_article), articleISEGuid);
            //Article:Line3
            Guid articleINTGuid = new Guid("bf99351b-1556-43c4-a85c-90082fb02d05");
            fin_article articleINT = (fin_article)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_article), articleINTGuid);
            //Article:Line4
            Guid articleNORGuid = new Guid("6b547918-769e-4f5b-bcd6-01af54846f73");
            fin_article articleNOR = (fin_article)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_article), articleNORGuid);
            //Place
            Guid placeGuid = new Guid("dd5a3869-db52-42d4-bbed-dec4adfaf62b");
            //Table
            Guid tableGuid = new Guid("64d417f6-ff97-4f4b-bded-4bc9bf9f18d9");

            //Get ArticleBag
            ArticleBag articleBag = new ArticleBag();
            articleBag.Add(articleRED, placeGuid, tableGuid, PriceType.Price1, 1.0m);
            articleBag.Add(articleISE, placeGuid, tableGuid, PriceType.Price1, 1.0m, vatExemptionReason);
            articleBag.Add(articleINT, placeGuid, tableGuid, PriceType.Price1, 1.0m);
            articleBag.Add(articleNOR, placeGuid, tableGuid, PriceType.Price1, 1.0m);

            //Prepare ProcessFinanceDocumentParameter
            ProcessFinanceDocumentParameter processFinanceDocumentParameter = new ProcessFinanceDocumentParameter(
                documentTypeGuid, articleBag)
            {
                Customer = customerGuid,
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag
            };
            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
        }

        void buttonPrintInvoiceDiscount_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = SettingsApp.XpoOidDocumentFinanceTypeInvoice;
            Guid customerGuid = new Guid("6223881a-4d2d-4de4-b254-f8529193da33");

            //Article:Line1
            Guid article1Guid = new Guid("72e8bde8-d03b-4637-90f1-fcb265658af0");
            fin_article article1 = (fin_article)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_article), article1Guid);
            //Article:Line2
            Guid article2Guid = new Guid("78638720-e728-4e96-8643-6d6267ff817b");
            fin_article article2 = (fin_article)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_article), article2Guid);
            //Place
            Guid placeGuid = new Guid("dd5a3869-db52-42d4-bbed-dec4adfaf62b");
            //Table
            Guid tableGuid = new Guid("64d417f6-ff97-4f4b-bded-4bc9bf9f18d9");

            //Get ArticleBag
            ArticleBag articleBag = new ArticleBag();
            articleBag.Add(article1, placeGuid, tableGuid, PriceType.Price1, 100.0m);
            articleBag.Add(article2, placeGuid, tableGuid, PriceType.Price1, 1.0m);

            //Prepare ProcessFinanceDocumentParameter
            ProcessFinanceDocumentParameter processFinanceDocumentParameter = new ProcessFinanceDocumentParameter(
              documentTypeGuid, articleBag)
            {
                Customer = customerGuid,
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag
            };
            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
        }

        void buttonPrintInvoiceExchangeRate_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = SettingsApp.XpoOidDocumentFinanceTypeInvoice;
            Guid customerGuid = new Guid("6223881a-4d2d-4de4-b254-f8529193da33");
            Guid currencyGuid = new Guid("28d692ad-0083-11e4-96ce-00ff2353398c");
            cfg_configurationcurrency currency = (cfg_configurationcurrency)GlobalFramework.SessionXpo.GetObjectByKey(typeof(cfg_configurationcurrency), currencyGuid);

            //Article:Line1
            Guid article1Guid = new Guid("72e8bde8-d03b-4637-90f1-fcb265658af0");
            fin_article article1 = (fin_article)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_article), article1Guid);
            //Article:Line2
            Guid article2Guid = new Guid("78638720-e728-4e96-8643-6d6267ff817b");
            fin_article article2 = (fin_article)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_article), article2Guid);
            //Place
            Guid placeGuid = new Guid("dd5a3869-db52-42d4-bbed-dec4adfaf62b");
            //Table
            Guid tableGuid = new Guid("64d417f6-ff97-4f4b-bded-4bc9bf9f18d9");

            //Get ArticleBag
            ArticleBag articleBag = new ArticleBag();
            articleBag.Add(article1, placeGuid, tableGuid, PriceType.Price1, 100.0m);
            articleBag.Add(article2, placeGuid, tableGuid, PriceType.Price1, 1.0m);

            //Prepare ProcessFinanceDocumentParameter
            ProcessFinanceDocumentParameter processFinanceDocumentParameter = new ProcessFinanceDocumentParameter(
              documentTypeGuid, articleBag)
            {
                Customer = customerGuid,
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag,
                Currency = currencyGuid,
                ExchangeRate = currency.ExchangeRate
            };
            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
        }

        void buttonPrintInvoiceJohnDoe1_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice;
            Guid customerGuid = new Guid("d8ce6455-e1a4-41dc-a475-223c00de3a91");//John Doe1

            //Article
            Guid article1Guid = new Guid("72e8bde8-d03b-4637-90f1-fcb265658af0");
            fin_article article1 = (fin_article)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_article), article1Guid);
            //Place
            Guid placeGuid = new Guid("dd5a3869-db52-42d4-bbed-dec4adfaf62b");
            //Table
            Guid tableGuid = new Guid("64d417f6-ff97-4f4b-bded-4bc9bf9f18d9");

            //Get ArticleBag
            ArticleBag articleBag = new ArticleBag();
            articleBag.Add(article1, placeGuid, tableGuid, PriceType.Price1, 1.0m);

            //Prepare ProcessFinanceDocumentParameter
            ProcessFinanceDocumentParameter processFinanceDocumentParameter = new ProcessFinanceDocumentParameter(
              documentTypeGuid, articleBag)
            {
                Customer = customerGuid,
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag
            };
            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
        }

        void buttonPrintInvoiceJohnDoe2_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice;
            Guid customerGuid = new Guid("f5a382bb-f826-40d8-8910-cfb18df8a41e");//John Doe2

            //Article
            Guid article1Guid = new Guid("32deb30d-ffa2-45e4-bca6-03569b9e8b08");
            fin_article article1 = (fin_article)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_article), article1Guid);
            //Place
            Guid placeGuid = new Guid("dd5a3869-db52-42d4-bbed-dec4adfaf62b");
            //Table
            Guid tableGuid = new Guid("64d417f6-ff97-4f4b-bded-4bc9bf9f18d9");

            //Get ArticleBag
            ArticleBag articleBag = new ArticleBag();
            articleBag.Add(article1, placeGuid, tableGuid, PriceType.Price1, 8.0m);

            //Prepare ProcessFinanceDocumentParameter
            ProcessFinanceDocumentParameter processFinanceDocumentParameter = new ProcessFinanceDocumentParameter(
              documentTypeGuid, articleBag)
            {
                Customer = customerGuid,
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag
            };
            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
        }

        void buttonPrintTransportationGuideWithTotals_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = new Guid("96bcf534-0dab-48bb-a69e-166e81ae6f7b");
            Guid customerGuid = new Guid("d64c5d26-b4f9-4220-bd3c-72ece5e3960a");

            //Article
            Guid article1Guid = new Guid("55892c3f-de10-4076-afde-619c54100c9b");
            fin_article article1 = (fin_article)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_article), article1Guid);
            //Place
            Guid placeGuid = new Guid("dd5a3869-db52-42d4-bbed-dec4adfaf62b");
            //Table
            Guid tableGuid = new Guid("64d417f6-ff97-4f4b-bded-4bc9bf9f18d9");

            //Get ArticleBag
            ArticleBag articleBag = new ArticleBag();
            articleBag.Add(article1, placeGuid, tableGuid, PriceType.Price1, 24.0m);

            //Prepare ProcessFinanceDocumentParameter
            ProcessFinanceDocumentParameter processFinanceDocumentParameter = new ProcessFinanceDocumentParameter(
              documentTypeGuid, articleBag)
            {
                Customer = customerGuid,
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag
            };
            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
        }

        void buttonPrintTransportationGuideWithoutTotals_Clicked(object sender, EventArgs e)
        {
            Guid documentTypeGuid = new Guid("96bcf534-0dab-48bb-a69e-166e81ae6f7b");
            Guid customerGuid = new Guid("6223881a-4d2d-4de4-b254-f8529193da33");

            //Article
            Guid article1Guid = new Guid("55892c3f-de10-4076-afde-619c54100c9b");
            fin_article article1 = (fin_article)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_article), article1Guid);
            //Place
            Guid placeGuid = new Guid("dd5a3869-db52-42d4-bbed-dec4adfaf62b");
            //Table
            Guid tableGuid = new Guid("64d417f6-ff97-4f4b-bded-4bc9bf9f18d9");

            //Get ArticleBag
            ArticleBag articleBag = new ArticleBag();
            articleBag.Add(article1, placeGuid, tableGuid, PriceType.Price1, 48.0m);

            //Prepare ProcessFinanceDocumentParameter
            ProcessFinanceDocumentParameter processFinanceDocumentParameter = new ProcessFinanceDocumentParameter(
              documentTypeGuid, articleBag)
            {
                Customer = customerGuid,
                SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag,
                ExchangeRate = 0.0m
            };
            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(SourceWindow, processFinanceDocumentParameter);
        }
    }
}

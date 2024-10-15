﻿using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Enums;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.shared.Enums;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Finance.DocumentProcessing;
using LogicPOS.Settings;
using LogicPOS.Shared.Article;
using LogicPOS.UI.Components;
using System;
using System.Data;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog
{
    internal class DocumentFinanceDialogPage3 : PagePadPage
    {
        private readonly Session _session;
        private readonly DocumentFinanceDialogPagePad _pagePad;
        private readonly PosDocumentFinanceDialog _posDocumentFinanceDialog;

        public ArticleBag ArticleBag { get; set; }
        public TreeViewDocumentFinanceArticle TreeViewArticles { get; set; }
        //UI Object References from other pages
        private DocumentFinanceDialogPage1 _pagePad1;
        //Required PagePage1 to be public to be assigned in PosDocumentFinanceDialog InitPages
        public DocumentFinanceDialogPage1 PagePad1
        {
            set { _pagePad1 = value; }
        }
        private DocumentFinanceDialogPage2 _pagePad2;
        //Required PagePage1 to be public to be assigned in PosDocumentFinanceDialog InitPages
        public DocumentFinanceDialogPage2 PagePad2
        {
            set { _pagePad2 = value; }
        }

        //Constructor
        public DocumentFinanceDialogPage3(Window parentWindow, string pPageName)
            : this(parentWindow, pPageName, "", null, true) { }
        public DocumentFinanceDialogPage3(Window parentWindow, string pPageName, Widget pWidget)
            : this(parentWindow, pPageName, "", pWidget, true) { }
        public DocumentFinanceDialogPage3(Window parentWindow, string pPageName, string pPageIcon, Widget pWidget, bool pEnabled = true)
            : base(parentWindow, pPageName, pPageIcon, pWidget, pEnabled)
        {
            //Init private vars
            _pagePad = (_sourceWindow as PosDocumentFinanceDialog).PagePad;
            _session = (_pagePad as DocumentFinanceDialogPagePad).Session;
            _posDocumentFinanceDialog = (_sourceWindow as PosDocumentFinanceDialog);

            //Init Tree
            TreeViewArticles = new TreeViewDocumentFinanceArticle(
              parentWindow,
              null,//DefaultValue 
              null,//DialogType
              GridViewMode.Default,
              GridViewNavigatorMode.Default
            );

            //Settings
            string fontGenericTreeViewFinanceDocumentArticleColumnTitle = AppSettings.Instance.fontGenericTreeViewFinanceDocumentArticleColumnTitle;
            string fontGenericTreeViewFinanceDocumentArticleColumn = AppSettings.Instance.fontGenericTreeViewFinanceDocumentArticleColumn;
            //Format Columns FontSizes for Touch
            TreeViewArticles.FormatColumnPropertiesForTouch(fontGenericTreeViewFinanceDocumentArticleColumnTitle, fontGenericTreeViewFinanceDocumentArticleColumn);
            //Disable View Button
            TreeViewArticles.Navigator.ButtonView.Sensitive = false;

            if (!GeneralSettings.AppUseBackOfficeMode) TreeViewArticles.Columns[18].Visible = false;

            PackStart(TreeViewArticles);

            //Events
            TreeViewArticles.RecordAfterInsert += delegate
            {
                //FORCE Assign FriendlyId to Designation
                fin_article article = (TreeViewArticles.Entity["Article.Code"] as fin_article);
                TreeViewArticlesRecordAfterChange();
            };
            TreeViewArticles.RecordAfterDelete += delegate { TreeViewArticlesRecordAfterChange(); };
            TreeViewArticles.RecordAfterUpdate += delegate { TreeViewArticlesRecordAfterChange(); };
        }

        private void TreeViewArticlesRecordAfterChange()
        {
            //Recreate ArticleBag
            ArticleBag = _posDocumentFinanceDialog.GetArticleBag();
            //Update Main Dialog Title
            _posDocumentFinanceDialog.WindowSettings.WindowTitle.Text = _posDocumentFinanceDialog.GetPageTitle(_pagePad.CurrentPageIndex);
            //Update Customer Edit Mode Fields
            _pagePad2.UpdateCustomerEditMode();
            //Validate this PagePad
            Validate();
            //TK016236 FrontOffice - Salvar sessão para novo documento 
            //GlobalFramework.SessionApp.CurrentOrderMainOid = currentOrderMain.Table.OrderMainOid;
            //GlobalFramework.SessionApp.Write();
        }

        //Override Base Validate
        public override void Validate()
        {
            //If not a Invoice|InvoiceAndPayment|Simplified Invoice
            if (TreeViewArticles.Entities.Rows.Count > 0
                && _pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid != InvoiceSettings.InvoiceId
                && _pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid != DocumentSettings.InvoiceAndPaymentId
                && _pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid != DocumentSettings.SimplifiedInvoiceId
            )
            {
                _validated = true;
            }
            //Check TotalFinal with Current Customer Details
            else if (
                TreeViewArticles.Entities.Rows.Count > 0 &&
                DocumentProcessingUtils.IsInValidFinanceDocumentCustomer(
                    ArticleBag.TotalFinal,
                    _pagePad2.EntryBoxSelectCustomerName.EntryValidation.Text,
                    _pagePad2.EntryBoxCustomerAddress.EntryValidation.Text,
                    _pagePad2.EntryBoxCustomerZipCode.EntryValidation.Text,
                    _pagePad2.EntryBoxCustomerCity.EntryValidation.Text,
                    _pagePad2.EntryBoxSelectCustomerCountry.EntryValidation.Text,
                    _pagePad2.EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text
                )
            )
            {
                logicpos.Utils.ShowMessageTouchSimplifiedInvoiceMaxValueExceedForFinalConsumer(_posDocumentFinanceDialog, ArticleBag.TotalFinal, GeneralSettings.GetRequiredCustomerDetailsAboveValue(XPOSettings.ConfigurationSystemCountry.Oid));
                _validated = false;
            }
            //If Simplified Invoice, Check if Total Document and Total Services is Not Greater than Max Value
            else if (
                TreeViewArticles.Entities.Rows.Count > 0 &&
                (
                    _pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid == DocumentSettings.SimplifiedInvoiceId
                    && (
                        //Check Total Final and Total Services
                        ArticleBag.TotalFinal > InvoiceSettings.GetSimplifiedInvoiceMaxItems(XPOSettings.ConfigurationSystemCountry.Oid) ||
                        ArticleBag.GetClassTotals("S") > InvoiceSettings.GetSimplifiedInvoiceMaxItems(XPOSettings.ConfigurationSystemCountry.Oid)
                    )
                )
            )
            {
                //In New Finance Dialog Mode we cant change to Invoice-Payment
                logicpos.Utils.ShowMessageTouchSimplifiedInvoiceMaxValueExceed(_posDocumentFinanceDialog, ShowMessageTouchSimplifiedInvoiceMaxValueExceedMode.DocumentFinanceDialog, ArticleBag.TotalFinal, InvoiceSettings.GetSimplifiedInvoiceMaxItems(XPOSettings.ConfigurationSystemCountry.Oid), ArticleBag.GetClassTotals("S"), InvoiceSettings.GetSimplifiedInvoiceMaxServices(XPOSettings.ConfigurationSystemCountry.Oid));
                _validated = false;
            }
            else
            {
                _validated = (TreeViewArticles.Entities.Rows.Count > 0);
            }

            //Enable Next Button, If not In Last Page and in WayBill Mode (WayBill + Invoice)
            if (_pagePad.CurrentPageIndex < _pagePad.Pages.Count - 1 && _pagePad.CurrentPageIndex == 2 && _pagePad.Pages[3].Enabled)
            {
                _pagePad.ButtonNext.Sensitive = _validated;
            }
            else
            {
                _pagePad.ButtonNext.Sensitive = false;
            };

            //Validate Dialog (All Pages must be Valid)
            _posDocumentFinanceDialog.Validate();
        }

        //Update TreeView TotalFinal, used when we change Customer Discount, this way we update Total Final for all Articles in TreeView
        public void UpdateTotalFinal()
        {
            bool debug = false;

            try
            {
                if (TreeViewArticles.Entities.Rows.Count > 0)
                {
                    fin_article article;
                    //Get Discount from Select Customer
                    decimal discountGlobal = LogicPOS.Utility.DataConversionUtils.StringToDecimal(_pagePad2.EntryBoxCustomerDiscount.EntryValidation.Text);
                    decimal exchangeRate = _pagePad1.EntryBoxSelectConfigurationCurrency.Value.ExchangeRate;

                    //Update DataTable Rows
                    foreach (DataRow item in TreeViewArticles.Entities.Rows)
                    {
                        article = XPOUtility.GetEntityById<fin_article>(new Guid(item.ItemArray[item.Table.Columns["Oid"].Ordinal].ToString()));

                        //Calc PriceProperties
                        PriceProperties priceProperties = PriceProperties.GetPriceProperties(
                            PricePropertiesSourceMode.FromPriceNet,
                            false,                                                                      //PriceWithVat
                            Convert.ToDecimal(item.ItemArray[item.Table.Columns["Price"].Ordinal]),     //Price
                            Convert.ToDecimal(item.ItemArray[item.Table.Columns["Quantity"].Ordinal]),  //Quantity
                            Convert.ToDecimal(item.ItemArray[item.Table.Columns["Discount"].Ordinal]),  //Discount
                            discountGlobal,
                            (item.ItemArray[item.Table.Columns["ConfigurationVatRate.Value"].Ordinal] as fin_configurationvatrate).Value //VatValue
                        );

                        //Finnally Update DataSourceRow Value with calculated PriceProperties
                        if (debug) _logger.Debug(string.Format("#1:TotalFinal DataSourceRow: [{0}], discountGlobal: [{1}]", LogicPOS.Utility.DataConversionUtils.DecimalToString(Convert.ToDecimal(TreeViewArticles.Entity["TotalFinal"])), LogicPOS.Utility.DataConversionUtils.DecimalToString(discountGlobal)));
                        //Update Display Values with ExchangeRate Multiplier
                        item["PriceDisplay"] = priceProperties.PriceNet * exchangeRate;
                        item["TotalNet"] = (priceProperties.TotalNet * exchangeRate);
                        item["TotalFinal"] = priceProperties.TotalFinal * exchangeRate;
                        item["PriceFinal"] = priceProperties.PriceFinal * exchangeRate;
                        if (debug) _logger.Debug(string.Format("#2:TotalFinal DataSourceRow: [{0}], discountGlobal: [{1}]", LogicPOS.Utility.DataConversionUtils.DecimalToString(Convert.ToDecimal(TreeViewArticles.Entity["TotalFinal"])), LogicPOS.Utility.DataConversionUtils.DecimalToString(discountGlobal)));
                    }
                    //Call Refresh, Recreate TreeView from Model
                    TreeViewArticles.Refresh();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}

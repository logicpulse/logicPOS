using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog
{
    class DocumentFinanceDialogPage3 : PagePadPage
    {
        private Session _session;
        private DocumentFinanceDialogPagePad _pagePad;
        private PosDocumentFinanceDialog _posDocumentFinanceDialog;

        //Public
        private ArticleBag _articleBag;
        public ArticleBag ArticleBag
        {
            get { return _articleBag; }
            set { _articleBag = value; }
        }
        //UI
        private TreeViewDocumentFinanceArticle _treeViewArticles;
        public TreeViewDocumentFinanceArticle TreeViewArticles
        {
            get { return _treeViewArticles; }
            set { _treeViewArticles = value; }
        }
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
        public DocumentFinanceDialogPage3(Window pSourceWindow, String pPageName) 
            : this(pSourceWindow, pPageName, "", null, true) { }
        public DocumentFinanceDialogPage3(Window pSourceWindow, String pPageName, Widget pWidget) 
            : this(pSourceWindow, pPageName, "", pWidget, true) { }
        public DocumentFinanceDialogPage3(Window pSourceWindow, String pPageName, String pPageIcon, Widget pWidget, bool pEnabled = true)
            : base(pSourceWindow, pPageName, pPageIcon, pWidget, pEnabled)
        {
            //Init private vars
            _pagePad = (_sourceWindow as PosDocumentFinanceDialog).PagePad;
            _session = (_pagePad as DocumentFinanceDialogPagePad).Session;
            _posDocumentFinanceDialog = (_sourceWindow as PosDocumentFinanceDialog);

            //Init Tree
            _treeViewArticles = new TreeViewDocumentFinanceArticle(
              pSourceWindow,
              null,//DefaultValue 
              null,//DialogType
              GenericTreeViewMode.Default,
              GenericTreeViewNavigatorMode.Default
            );

            //Settings
            string fontGenericTreeViewFinanceDocumentArticleColumnTitle = GlobalFramework.Settings["fontGenericTreeViewFinanceDocumentArticleColumnTitle"];
            string fontGenericTreeViewFinanceDocumentArticleColumn = GlobalFramework.Settings["fontGenericTreeViewFinanceDocumentArticleColumn"];
            //Format Columns FontSizes for Touch
            _treeViewArticles.FormatColumnPropertiesForTouch(fontGenericTreeViewFinanceDocumentArticleColumnTitle, fontGenericTreeViewFinanceDocumentArticleColumn);
            //Disable View Button
            _treeViewArticles.Navigator.ButtonView.Sensitive = false;

            PackStart(_treeViewArticles);

            //Events
            _treeViewArticles.RecordAfterInsert += delegate
            {
                //FORCE Assign FriendlyId to Designation
                fin_article article = (_treeViewArticles.DataSourceRow["Article.Code"] as fin_article);
                treeViewArticlesRecordAfterChange();
            };
            _treeViewArticles.RecordAfterDelete += delegate { treeViewArticlesRecordAfterChange(); };
            _treeViewArticles.RecordAfterUpdate += delegate { treeViewArticlesRecordAfterChange(); };
        }

        private void treeViewArticlesRecordAfterChange()
        {
            //Recreate ArticleBag
            _articleBag = _posDocumentFinanceDialog.GetArticleBag();
            //Update Main Dialog Title
            _posDocumentFinanceDialog.WindowTitle = _posDocumentFinanceDialog.GetPageTitle(_pagePad.CurrentPageIndex);
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
            if (_treeViewArticles.DataSource.Rows.Count > 0 
                && _pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid != SettingsApp.XpoOidDocumentFinanceTypeInvoice 
                && _pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid != SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment 
                && _pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid != SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice
            )
            {
                _validated = true;
            }
            //Check TotalFinal with Current Customer Details
            else if (
                _treeViewArticles.DataSource.Rows.Count > 0 &&
                FrameworkUtils.IsInValidFinanceDocumentCustomer(
                    _articleBag.TotalFinal, 
                    _pagePad2.EntryBoxSelectCustomerName.EntryValidation.Text,
                    _pagePad2.EntryBoxCustomerAddress.EntryValidation.Text,
                    _pagePad2.EntryBoxCustomerZipCode.EntryValidation.Text,
                    _pagePad2.EntryBoxCustomerCity.EntryValidation.Text,
                    _pagePad2.EntryBoxSelectCustomerCountry.EntryValidation.Text,
                    _pagePad2.EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text
                )
            )
            {
                Utils.ShowMessageTouchSimplifiedInvoiceMaxValueExceedForFinalConsumer(_posDocumentFinanceDialog, _articleBag.TotalFinal, SettingsApp.FinanceRuleRequiredCustomerDetailsAboveValue);
                _validated = false;
            }
            //If Simplified Invoice, Check if Total Document and Total Services is Not Greater than Max Value
            else if (
                _treeViewArticles.DataSource.Rows.Count > 0 &&
                (
                    _pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid == SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice 
                    && (
                        //Check Total Final and Total Services
                        _articleBag.TotalFinal > SettingsApp.FinanceRuleSimplifiedInvoiceMaxTotal ||
                        _articleBag.GetClassTotals("S") > SettingsApp.FinanceRuleSimplifiedInvoiceMaxTotalServices
                    )
                )
            )
            {
                //In New Finance Dialog Mode we cant change to Invoice-Payment
                Utils.ShowMessageTouchSimplifiedInvoiceMaxValueExceed(_posDocumentFinanceDialog, ShowMessageTouchSimplifiedInvoiceMaxValueExceedMode.DocumentFinanceDialog, _articleBag.TotalFinal, SettingsApp.FinanceRuleSimplifiedInvoiceMaxTotal, _articleBag.GetClassTotals("S"), SettingsApp.FinanceRuleSimplifiedInvoiceMaxTotalServices);
                _validated = false;
            }
            else
            {
                _validated = (_treeViewArticles.DataSource.Rows.Count > 0);
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
                if (_treeViewArticles.DataSource.Rows.Count > 0)
                {
                    fin_article article;
                    //Get Discount from Select Customer
                    decimal discountGlobal = FrameworkUtils.StringToDecimal(_pagePad2.EntryBoxCustomerDiscount.EntryValidation.Text);
                    decimal exchangeRate = _pagePad1.EntryBoxSelectConfigurationCurrency.Value.ExchangeRate;

                    //Update DataTable Rows
                    foreach (DataRow item in _treeViewArticles.DataSource.Rows)
                    {
                        article = (fin_article)FrameworkUtils.GetXPGuidObject(typeof(fin_article), new Guid(item.ItemArray[item.Table.Columns["Oid"].Ordinal].ToString()));

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
                        if (debug) _log.Debug(string.Format("#1:TotalFinal DataSourceRow: [{0}], discountGlobal: [{1}]", FrameworkUtils.DecimalToString(Convert.ToDecimal(_treeViewArticles.DataSourceRow["TotalFinal"])), FrameworkUtils.DecimalToString(discountGlobal)));
                        //Update Display Values with ExchangeRate Multiplier
                        item["PriceDisplay"] = priceProperties.PriceNet * exchangeRate;
                        item["TotalNet"] = (priceProperties.TotalNet * exchangeRate);
                        item["TotalFinal"] = priceProperties.TotalFinal * exchangeRate;
                        item["PriceFinal"] = priceProperties.PriceFinal * exchangeRate;
                        if (debug) _log.Debug(string.Format("#2:TotalFinal DataSourceRow: [{0}], discountGlobal: [{1}]", FrameworkUtils.DecimalToString(Convert.ToDecimal(_treeViewArticles.DataSourceRow["TotalFinal"])), FrameworkUtils.DecimalToString(discountGlobal)));
                    }
                    //Call Refresh, Recreate TreeView from Model
                    _treeViewArticles.Refresh();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}

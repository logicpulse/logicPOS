using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using System;
using System.Collections.Generic;
using System.Drawing;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Domain.Entities;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosDocumentFinanceDialog : PosBaseDialog
    {
        //Private
        private List<PagePadPage> _listPages;
        private DocumentFinanceDialogPage1 _pagePad1;
        private DocumentFinanceDialogPage2 _pagePad2;
        private DocumentFinanceDialogPage3 _pagePad3;
        private DocumentFinanceDialogPage4 _pagePad4;
        private DocumentFinanceDialogPage5 _pagePad5;

        public TouchButtonIconWithText ButtonClearCustomer { get; }
        private readonly TouchButtonIconWithText _buttonOk;
        private readonly TouchButtonIconWithText _buttonCancel;
        private readonly TouchButtonIconWithText _buttonPreview;
        //Custom Responses Types
        private readonly ResponseType _responseTypePreview = (ResponseType)11;
        private readonly ResponseType _responseTypeClearCustomer = (ResponseType)12;

        public Dictionary<Guid, decimal> ValidateMaxQuantities { get; set; }

        public DocumentFinanceDialogPagePad PagePad { get; set; }

        public PosDocumentFinanceDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : base(pSourceWindow, pDialogFlags)
        {
            //Parameters
            _sourceWindow = pSourceWindow;
            //Init Local Vars
            Size windowSize = new Size(790, 546);
            //Image Icons
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_document_new.png";
            string fileActionPreview = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_preview.png";
            string fileIconClearCustomer = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_nav_delete.png";

            InitPages();

            //Init Content
            //Fixed fixedContent = new Fixed();
            VBox boxContent = new VBox();
            boxContent.PackStart(PagePad, true, true, 0);

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
            ButtonClearCustomer = ActionAreaButton.FactoryGetDialogButtonType("touchButtonClearCustomer_DialogActionArea", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_button_label_payment_dialog_clear_client"), fileIconClearCustomer);

            _buttonPreview = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPreview_DialogActionArea", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "widget_generictreeviewnavigator_preview"), fileActionPreview); /* IN009111 */
            _buttonOk.Sensitive = false;

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(ButtonClearCustomer, _responseTypeClearCustomer),
                new ActionAreaButton(_buttonPreview, _responseTypePreview),
                new ActionAreaButton(_buttonOk, ResponseType.Ok),
                new ActionAreaButton(_buttonCancel, ResponseType.Cancel)
            };

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, _windowTitle, windowSize, boxContent, actionAreaButtons);

            //Hide After Init Show All
            ButtonClearCustomer.Visible = false;
            _buttonPreview.Visible = false;
            //Hide Pages
            PagePad.Pages[3].NavigatorButton.Visible = false;
            PagePad.Pages[4].NavigatorButton.Visible = false;
        }

        private void InitPages()
        {
            //Init here before Creating Pages to Have PagePad Constructed for PagePadPage
            PagePad = new DocumentFinanceDialogPagePad(this);
            PagePad.PageChanged += _pagePad_PageChanged;

            _listPages = new List<PagePadPage>();
            //Assign Page Title
            _windowTitle = GetPageTitle(0);

            string icon1 = PathsSettings.ImagesFolderLocation + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_1_new_document.png";
            string icon2 = PathsSettings.ImagesFolderLocation + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_2_customer.png";
            string icon3 = PathsSettings.ImagesFolderLocation + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_3_article.png";
            string icon4 = PathsSettings.ImagesFolderLocation + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_4_waybill_to.png";
            string icon5 = PathsSettings.ImagesFolderLocation + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_5_waybill_from.png";

            _pagePad1 = new DocumentFinanceDialogPage1(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_document_finance_page1"), icon1, null);
            _pagePad2 = new DocumentFinanceDialogPage2(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_document_finance_page2"), icon2, null);
            _pagePad3 = new DocumentFinanceDialogPage3(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_document_finance_page3"), icon3, null);
            //Start in Invoice : Start Disabled
            _pagePad4 = new DocumentFinanceDialogPage4(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_document_finance_page4"), icon4, null, false);
            _pagePad5 = new DocumentFinanceDialogPage5(this, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_document_finance_page5"), icon5, null, false);
            //Assign Reference Here, After Construction
            //PagePad
            _pagePad1.PagePad2 = _pagePad2;
            _pagePad1.PagePad3 = _pagePad3;
            _pagePad1.PagePad4 = _pagePad4;
            _pagePad1.PagePad5 = _pagePad5;
            //PagePad2
            _pagePad2.PagePad1 = _pagePad1;
            _pagePad2.PagePad3 = _pagePad3;
            _pagePad2.PagePad4 = _pagePad4;
            _pagePad2.PagePad5 = _pagePad5;
            //PagePad3
            _pagePad3.PagePad1 = _pagePad1;
            _pagePad3.PagePad2 = _pagePad2;
            //Developer
            //PagePadPage page6 = new DocumentFinanceDialogPage6(this, "Page6");
            //PagePadPage page7 = new DocumentFinanceDialogPage7(this, "Page7");
            //PagePadPage page8 = new DocumentFinanceDialogPage8(this, "Page8");

            //Call required Methods after Constructed pages
            _pagePad2.ApplyCriteriaToCustomerInputs();

            _listPages.Add(_pagePad1);
            _listPages.Add(_pagePad2);
            _listPages.Add(_pagePad3);
            _listPages.Add(_pagePad4);
            _listPages.Add(_pagePad5);

            //Developer
            //listPages.Add(page6);
            //listPages.Add(page7);
            //listPages.Add(page8);

            //Init PagePage
            PagePad.Init(_listPages);

            //Start Validated
            _pagePad1.Validate();

            //Prepare Other Pages - Enable/Disable WayBill Required Validation Entrys, Start with Disabled Validation for Invoices (Optional Mode)
            _pagePad1.ToggleWayBillValidation(false);
            //When Start in Invoice Mode, Start with Ship From Assigned, if disabled Invoice has WayBill comment above line
            _pagePad5.AssignShipFromDefaults();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        private void _pagePad_PageChanged(object sender, EventArgs e)
        {
            this.WindowTitle = GetPageTitle(PagePad.CurrentPageIndex);
            PagePad.ActivePage.Validate();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Methods

        public string GetPageTitle(int pPageIndex)
        {
            string result = string.Format("{0} :: {1}",
  CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_new_finance_document"),
  CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, string.Format("window_title_dialog_document_finance_page{0}", pPageIndex + 1))
);

            //Enable/Disable ClearCustomer
            if (ButtonClearCustomer != null) ButtonClearCustomer.Visible = (_pagePad2 != null && pPageIndex == 1);

            //Enable/Disable Preview
            if (_pagePad3 != null && _pagePad3.ArticleBag != null)
            {
                //Reference
                cfg_configurationcurrency configurationCurrency = _pagePad1.EntryBoxSelectConfigurationCurrency.Value;

                //Always Update Totals before Show Title
                _pagePad3.ArticleBag.DiscountGlobal = LogicPOS.Utility.DataConversionUtils.StringToDecimal(_pagePad2.EntryBoxCustomerDiscount.EntryValidation.Text);
                _pagePad3.ArticleBag.UpdateTotals();

                if (_pagePad3.ArticleBag.TotalFinal > 0)
                {
                    //Always Recreate ArticleBag before contruct ProcessFinanceDocumentParameter
                    _pagePad3.ArticleBag = GetArticleBag();
                    result += string.Format(" : {0}", LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(_pagePad3.ArticleBag.TotalFinal * configurationCurrency.ExchangeRate, configurationCurrency.Acronym));
                    //Enable or Disabled Preview Button
                    _buttonPreview.Visible = true;
                }
                else
                {
                    //Enable or Disabled Preview Button
                    _buttonPreview.Visible = false;
                }
            }

            return result;
        }

        public bool Validate()
        {
            bool result = true;

            for (int i = 0; i < _listPages.Count; i++)
            {
                if (_listPages[i].Enabled)
                {
                    //_logger.Debug(string.Format("listPages[{0}].Enabled: [{1}], Validated[{2}]", i, _listPages[i].Enabled, _listPages[i].Validated));
                    //If Enabled and Not Validated return False
                    if (!_listPages[i].Validated) result = false;
                }
            }

            if (_buttonOk != null) _buttonOk.Sensitive = result;

            return result;
        }
    }
}

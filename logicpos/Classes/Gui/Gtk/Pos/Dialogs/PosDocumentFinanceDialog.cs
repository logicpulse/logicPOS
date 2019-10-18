using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    partial class PosDocumentFinanceDialog : PosBaseDialog
    {
        //Private
        private List<PagePadPage> _listPages;
        private DocumentFinanceDialogPage1 _pagePad1;
        private DocumentFinanceDialogPage2 _pagePad2;
        private DocumentFinanceDialogPage3 _pagePad3;
        private DocumentFinanceDialogPage4 _pagePad4;
        private DocumentFinanceDialogPage5 _pagePad5;
        //UI
        private TouchButtonIconWithText _buttonClearCustomer;
        public TouchButtonIconWithText ButtonClearCustomer { 
            get { return _buttonClearCustomer; } 
        }
        private TouchButtonIconWithText _buttonOk;
        private TouchButtonIconWithText _buttonCancel;
        private TouchButtonIconWithText _buttonPreview;
        //Custom Responses Types
        private ResponseType _responseTypePreview = (ResponseType)11;
        private ResponseType _responseTypeClearCustomer = (ResponseType)12;
        //DocumentFinanceArticle MaxQuantities Validate
        private Dictionary<Guid,decimal> _validateMaxQuantities;
        public Dictionary<Guid,decimal> ValidateMaxQuantities
        {
            get { return _validateMaxQuantities; }
            set { _validateMaxQuantities = value; }
        }

        //Public
        private DocumentFinanceDialogPagePad _pagePad;
        public DocumentFinanceDialogPagePad PagePad
        {
            get { return _pagePad; }
            set { _pagePad = value; }
        }

        public PosDocumentFinanceDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : base(pSourceWindow, pDialogFlags)
        {
            //Parameters
            _sourceWindow = pSourceWindow;
            //Init Local Vars
            Size windowSize = new Size(780, 546);
            //Image Icons
            string fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_document_new.png");
            string fileActionPreview = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_preview.png");
            string fileIconClearCustomer = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_nav_delete.png");

            InitPages();

            //Init Content
            //Fixed fixedContent = new Fixed();
            VBox boxContent = new VBox();
            boxContent.PackStart(_pagePad, true, true, 0);

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
            _buttonClearCustomer = ActionAreaButton.FactoryGetDialogButtonType("touchButtonClearCustomer_DialogActionArea", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_payment_dialog_clear_client"), fileIconClearCustomer);

            _buttonPreview = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPreview_DialogActionArea", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "widget_generictreeviewnavigator_preview"), fileActionPreview); /* IN009111 */
            _buttonOk.Sensitive = false;

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            actionAreaButtons.Add(new ActionAreaButton(_buttonClearCustomer, _responseTypeClearCustomer));
            actionAreaButtons.Add(new ActionAreaButton(_buttonPreview, _responseTypePreview));
            actionAreaButtons.Add(new ActionAreaButton(_buttonOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(_buttonCancel, ResponseType.Cancel));

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, _windowTitle, windowSize, boxContent, actionAreaButtons);

            //Hide After Init Show All
            _buttonClearCustomer.Visible = false;
            _buttonPreview.Visible = false;
            //Hide Pages
            _pagePad.Pages[3].NavigatorButton.Visible = false;
            _pagePad.Pages[4].NavigatorButton.Visible = false;
        }

        private void InitPages()
        {
            //Init here before Creating Pages to Have PagePad Constructed for PagePadPage
            _pagePad = new DocumentFinanceDialogPagePad(this);
            _pagePad.PageChanged += _pagePad_PageChanged;

            _listPages = new List<PagePadPage>();
            //Assign Page Title
            _windowTitle = GetPageTitle(0);

            string icon1 = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_1_new_document.png");
            string icon2 = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_2_customer.png");
            string icon3 = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_3_article.png");
            string icon4 = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_4_waybill_to.png");
            string icon5 = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_5_waybill_from.png");

            _pagePad1 = new DocumentFinanceDialogPage1(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_document_finance_page1"), icon1, null);
            _pagePad2 = new DocumentFinanceDialogPage2(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_document_finance_page2"), icon2, null);
            _pagePad3 = new DocumentFinanceDialogPage3(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_document_finance_page3"), icon3, null);
            //Start in Invoice : Start Disabled
            _pagePad4 = new DocumentFinanceDialogPage4(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_document_finance_page4"), icon4, null, false);
            _pagePad5 = new DocumentFinanceDialogPage5(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_document_finance_page5"), icon5, null, false);
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
            _pagePad.Init(_listPages);

            //Start Validated
            _pagePad1.Validate();

            //Prepare Other Pages - Enable/Disable WayBill Required Validation Entrys, Start with Disabled Validation for Invoices (Optional Mode)
            _pagePad1.ToggleWayBillValidation(false);
            //When Start in Invoice Mode, Start with Ship From Assigned, if disabled Invoice has WayBill comment above line
            _pagePad5.AssignShipFromDefaults();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        void _pagePad_PageChanged(object sender, EventArgs e)
        {
            this.WindowTitle = GetPageTitle(_pagePad.CurrentPageIndex);
            _pagePad.ActivePage.Validate();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Methods

        public string GetPageTitle(int pPageIndex)
        {
            string result = string.Empty;

            result = string.Format("{0} :: {1}",
              resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_new_finance_document"),
              resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], string.Format("window_title_dialog_document_finance_page{0}", pPageIndex + 1))
            );

            //Enable/Disable ClearCustomer
            if (_buttonClearCustomer != null) _buttonClearCustomer.Visible = (_pagePad2 != null && pPageIndex == 1);

            //Enable/Disable Preview
            if (_pagePad3 != null && _pagePad3.ArticleBag != null)
            {
                //Reference
                cfg_configurationcurrency configurationCurrency = _pagePad1.EntryBoxSelectConfigurationCurrency.Value;

                //Always Update Totals before Show Title
                _pagePad3.ArticleBag.DiscountGlobal = FrameworkUtils.StringToDecimal(_pagePad2.EntryBoxCustomerDiscount.EntryValidation.Text);
                _pagePad3.ArticleBag.UpdateTotals();

                if (_pagePad3.ArticleBag.TotalFinal > 0)
                {
                    result += string.Format(" : {0}", FrameworkUtils.DecimalToStringCurrency(_pagePad3.ArticleBag.TotalFinal * configurationCurrency.ExchangeRate, configurationCurrency.Acronym));
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
                    //_log.Debug(string.Format("listPages[{0}].Enabled: [{1}], Validated[{2}]", i, _listPages[i].Enabled, _listPages[i].Validated));
                    //If Enabled and Not Validated return False
                    if (!_listPages[i].Validated) result = false;
                }
            }

            if (_buttonOk != null) _buttonOk.Sensitive = result;

            return result;
        }
    }
}

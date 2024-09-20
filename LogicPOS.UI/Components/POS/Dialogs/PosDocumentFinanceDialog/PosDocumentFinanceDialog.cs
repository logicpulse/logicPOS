using Gtk;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog;
using logicpos.Classes.Gui.Gtk.Widgets;
using System;
using System.Collections.Generic;
using System.Drawing;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Utility;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Buttons;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosDocumentFinanceDialog : BaseDialog
    {
        private List<PagePadPage> Pages { get; set; }
        private DocumentFinanceDialogPage1 Page1 { get; set; }
        private DocumentFinanceDialogPage2 Page2 { get; set; }
        private DocumentFinanceDialogPage3 Page3 { get; set; }
        private DocumentFinanceDialogPage4 Page4 { get; set; }
        private DocumentFinanceDialogPage5 Page5 { get; set; }

        public IconButtonWithText ButtonClearCustomer { get; }
        private readonly IconButtonWithText _buttonOk;
        private readonly IconButtonWithText _buttonCancel;
        private readonly IconButtonWithText _buttonPreview;

        private readonly ResponseType _responseTypePreview = (ResponseType)11;
        private readonly ResponseType _responseTypeClearCustomer = (ResponseType)12;

        public Dictionary<Guid, decimal> ValidateMaxQuantities { get; set; }

        public DocumentFinanceDialogPagePad PagePad { get; set; }

        public PosDocumentFinanceDialog(Window parent, DialogFlags flags)
            : base(parent, flags)
        {
            WindowSettings.Source = parent;
            Size windowSize = new Size(790, 546);

            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_document_new.png";
            string fileActionPreview = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_preview.png";
            string fileIconClearCustomer = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_nav_delete.png";

            InitPages();

            VBox boxContent = new VBox();
            boxContent.PackStart(PagePad, true, true, 0);

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
            ButtonClearCustomer = ActionAreaButton.FactoryGetDialogButtonType("touchButtonClearCustomer_DialogActionArea", GeneralUtils.GetResourceByName("global_button_label_payment_dialog_clear_client"), fileIconClearCustomer);

            _buttonPreview = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPreview_DialogActionArea", GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_preview"), fileActionPreview); /* IN009111 */
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
            this.Initialize(this, flags, fileDefaultWindowIcon, WindowSettings.WindowTitle.Text, windowSize, boxContent, actionAreaButtons);

            //Hide After Init Show All
            ButtonClearCustomer.Visible = false;
            _buttonPreview.Visible = false;
            //Hide Pages
            PagePad.Pages[3].NavigatorButton.Visible = false;
            PagePad.Pages[4].NavigatorButton.Visible = false;
        }

        private void InitPages()
        {

            PagePad = new DocumentFinanceDialogPagePad(this);
            PagePad.PageChanged += _pagePad_PageChanged;

            Pages = new List<PagePadPage>();
            WindowSettings.WindowTitle.Text = GetPageTitle(0);

            string icon1 = PathsSettings.ImagesFolderLocation + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_1_new_document.png";
            string icon2 = PathsSettings.ImagesFolderLocation + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_2_customer.png";
            string icon3 = PathsSettings.ImagesFolderLocation + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_3_article.png";
            string icon4 = PathsSettings.ImagesFolderLocation + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_4_waybill_to.png";
            string icon5 = PathsSettings.ImagesFolderLocation + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_5_waybill_from.png";

            Page1 = new DocumentFinanceDialogPage1(this, GeneralUtils.GetResourceByName("window_title_dialog_document_finance_page1"), icon1, null);
            Page2 = new DocumentFinanceDialogPage2(this, GeneralUtils.GetResourceByName("window_title_dialog_document_finance_page2"), icon2, null);
            Page3 = new DocumentFinanceDialogPage3(this, GeneralUtils.GetResourceByName("window_title_dialog_document_finance_page3"), icon3, null);

            Page4 = new DocumentFinanceDialogPage4(this, GeneralUtils.GetResourceByName("window_title_dialog_document_finance_page4"), icon4, null, false);
            Page5 = new DocumentFinanceDialogPage5(this, GeneralUtils.GetResourceByName("window_title_dialog_document_finance_page5"), icon5, null, false);

            Page1.PagePad2 = Page2;
            Page1.PagePad3 = Page3;
            Page1.PagePad4 = Page4;
            Page1.PagePad5 = Page5;

            Page2.PagePad1 = Page1;
            Page2.PagePad3 = Page3;
            Page2.PagePad4 = Page4;
            Page2.PagePad5 = Page5;

            Page3.PagePad1 = Page1;
            Page3.PagePad2 = Page2;

            Page2.ApplyCriteriaToCustomerInputs();

            Pages.Add(Page1);
            Pages.Add(Page2);
            Pages.Add(Page3);
            Pages.Add(Page4);
            Pages.Add(Page5);

            PagePad.Init(Pages);

            Page1.Validate();

            Page1.ToggleWayBillValidation(false);
            Page5.AssignShipFromDefaults();
        }


        private void _pagePad_PageChanged(object sender, EventArgs e)
        {
            this.WindowSettings.WindowTitle.Text = GetPageTitle(PagePad.CurrentPageIndex);
            PagePad.ActivePage.Validate();
        }


        public string GetPageTitle(int pPageIndex)
        {
            string result = $"{GeneralUtils.GetResourceByName("window_title_dialog_new_finance_document")} :: {CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, string.Format("window_title_dialog_document_finance_page{0}", pPageIndex + 1))}";

            if (ButtonClearCustomer != null) ButtonClearCustomer.Visible = (Page2 != null && pPageIndex == 1);

            if (Page3 != null && Page3.ArticleBag != null)
            {
                cfg_configurationcurrency configurationCurrency = Page1.EntryBoxSelectConfigurationCurrency.Value;

                Page3.ArticleBag.DiscountGlobal = DataConversionUtils.StringToDecimal(Page2.EntryBoxCustomerDiscount.EntryValidation.Text);
                Page3.ArticleBag.UpdateTotals();

                if (Page3.ArticleBag.TotalFinal > 0)
                {
                    Page3.ArticleBag = GetArticleBag();
                    result += string.Format(" : {0}", DataConversionUtils.DecimalToStringCurrency(Page3.ArticleBag.TotalFinal * configurationCurrency.ExchangeRate, configurationCurrency.Acronym));
                    _buttonPreview.Visible = true;
                }
                else
                {
                    _buttonPreview.Visible = false;
                }
            }

            return result;
        }

        public bool Validate()
        {
            bool result = true;

            for (int i = 0; i < Pages.Count; i++)
            {
                if (Pages[i].Enabled)
                {
                    if (!Pages[i].Validated) result = false;
                }
            }

            if (_buttonOk != null) _buttonOk.Sensitive = result;

            return result;
        }
    }
}

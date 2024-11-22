using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using Gtk;
using LogicPOS.UI.Extensions;
using System.Drawing;
using LogicPOS.UI.Components.Menus;


namespace LogicPOS.UI.Components.POS
{
    public partial class PaymentsModal
    {
        #region Components
        private IconButtonWithText BtnOk { get; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnClearCustomer { get; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonClearCustomer_DialogActionArea",
                                                                                                           GeneralUtils.GetResourceByName("global_button_label_payment_dialog_clear_client"),
                                                                                                           PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_nav_delete.png");
        private IconButtonWithText BtnNewCustomer { get; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonClearCustomer_DialogActionArea",
                                                                                                          GeneralUtils.GetResourceByName("dialog_button_label_new_client"),
                                                                                                          PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_clients.png");
        private IconButtonWithText BtnFullPayment { get; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonFullPayment_DialogActionArea",
                                                                                                          GeneralUtils.GetResourceByName("global_button_label_payment_dialog_full_payment"),
                                                                                                          PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_full.png");
        private IconButtonWithText BtnPartialPayment { get; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPartialPayment_DialogActionArea",
                                                                                                            GeneralUtils.GetResourceByName("global_button_label_payment_dialog_partial_payment"),
                                                                                                            PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_partial.png");
        private IconButtonWithText BtnInvoice { get; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPartialPayment_DialogActionArea",
                                                                                                                   GeneralUtils.GetResourceByName("global_documentfinance_type_title_ft"),
                                                                                                                 PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_finance_document.png");

        private IconButton BtnPrevious { get; set; }
        private IconButton BtnNext { get; set; }

        private PaymentMethodsMenu PaymentMethodsMenu { get; set; }

        private PageTextBox TxtCustomer { get; set; }
        private PageTextBox TxtFiscalNumber { get; set; }
        private PageTextBox TxtCountry { get; set; }
        private PageTextBox TxtDiscount { get; set; }
        private PageTextBox TxtCardNumber { get; set; }
        private PageTextBox TxtAddress { get; set; }
        private PageTextBox TxtLocality { get; set; }
        private PageTextBox TxtZipCode { get; set; }
        private PageTextBox TxtCity { get; set; }
        private PageTextBox TxtNotes { get; set; }

        public HashSet<IValidatableField> ValidatableFields { get; private set; } = new HashSet<IValidatableField>();

        private Label LabelTotal { get; } = new Label(GeneralUtils.GetResourceByName("global_total_price_to_pay") + ":");
        private Label LabelDelivery { get; } = new Label(GeneralUtils.GetResourceByName("global_total_deliver") + ":");
        private Label LabelChange { get; } = new Label(GeneralUtils.GetResourceByName("global_total_change") + ":");
        private Label LabelTotalValue { get; } = new Label("0");
        private Label LabelDeliveryValue { get; } = new Label("0");
        private Label LabelChangeValue { get; } = new Label("0");
        #endregion

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            InitializeButtons();
            InitializeLabels();

            return new ActionAreaButtons
                {
                    new ActionAreaButton(BtnInvoice, ResponseType.None),
                    new ActionAreaButton(BtnClearCustomer, ResponseType.None),
                    new ActionAreaButton(BtnNewCustomer, ResponseType.None),
                    new ActionAreaButton(BtnFullPayment, ResponseType.None),
                    new ActionAreaButton(BtnPartialPayment, ResponseType.None),
                    new ActionAreaButton(BtnOk, ResponseType.Ok),
                    new ActionAreaButton(BtnCancel, ResponseType.Cancel)
                };
        }

        protected override Widget CreateBody()
        {
            InitializeTextFields();
            VBox verticalLayout = new VBox(false, 0);
            verticalLayout.PackStart(CreateTotalsTable(), true, true, 0);
            verticalLayout.PackStart(CreatePaymentMethodsPanel(), true, true, 0);
            verticalLayout.PackStart(PageTextBox.CreateHbox(TxtFiscalNumber, TxtCardNumber), true, true, 0);
            verticalLayout.PackStart(PageTextBox.CreateHbox(TxtCustomer, TxtDiscount), true, true, 0);
            verticalLayout.PackStart(TxtAddress.Component, true, true, 0);
            verticalLayout.PackStart(TxtLocality.Component, true, true, 0);
            verticalLayout.PackStart(PageTextBox.CreateHbox(TxtZipCode, TxtCity, TxtCountry), true, true, 0);
            verticalLayout.PackStart(TxtNotes.Component, true, true, 0);

            return verticalLayout;
        }

        private HBox CreatePaymentMethodsPanel()
        {
            HBox hbox = new HBox(false, 0);
            PaymentMethodsMenu = new PaymentMethodsMenu(BtnPrevious, BtnNext);
            PaymentMethodsMenu.PaymentMethodSelected += PaymentMethodSelected;
            hbox.HeightRequest = AppSettings.Instance.sizeBaseDialogDefaultButton.Height + 10;
            hbox.PackStart(BtnPrevious, false, false, 0);
            hbox.PackStart(PaymentMethodsMenu, true, true, 0);
            hbox.PackEnd(BtnNext, false, false, 0);
            return hbox;
        }

        private EventBox CreateTotalsTable()
        {
            uint padding = 5;
            Gtk.Table table = new Gtk.Table(3, 2, false);
            table.HeightRequest = 100;

            //Row 1
            table.Attach(LabelTotal, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(LabelTotalValue, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);

            //Row 2
            table.Attach(LabelDelivery, 0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(LabelDeliveryValue, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);

            //Row 3
            table.Attach(LabelChange, 0, 1, 2, 3, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(LabelChangeValue, 1, 2, 2, 3, AttachOptions.Fill, AttachOptions.Fill, padding, padding);

            EventBox eventBox = new EventBox();
            eventBox.BorderWidth = 2;
            eventBox.ModifyBg(StateType.Normal, AppSettings.Instance.colorPosPaymentsDialogTotalPannelBackground.ToGdkColor());
            eventBox.Add(table);

            return eventBox;
        }

        private void InitializeTextFields()
        {
            InitializeTxtCustomer();
            InitializeTxtFiscalNumber();
            InitializeTxtCardNumber();
            InitializeTxtDiscount();
            InitializeTxtAddress();
            InitializeTxtLocality();
            InitializeTxtZipCode();
            InitializeTxtCity();
            InitializeTxtCountry();
            InitializeTxtNotes();
        }

        private void InitializeTxtCountry()
        {
            TxtCountry = new PageTextBox(this,
                                         GeneralUtils.GetResourceByName("global_country"),
                                         isRequired: true,
                                         isValidatable: false,
                                         includeSelectButton: true,
                                         includeKeyBoardButton: false);

            TxtCountry.Entry.IsEditable = false;
            TxtCountry.SelectEntityClicked += BtnSelectCountry_Clicked;
            ValidatableFields.Add(TxtCountry);
        }

        private void InitializeTxtCity()
        {
            TxtCity = new PageTextBox(this,
                                      GeneralUtils.GetResourceByName("global_city"),
                                      isRequired: false,
                                      isValidatable: false,
                                      includeSelectButton: false,
                                      includeKeyBoardButton: true);
        }

        private void InitializeTxtZipCode()
        {
            TxtZipCode = new PageTextBox(this,
                                         GeneralUtils.GetResourceByName("global_zipcode"),
                                         isRequired: false,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: true);
        }

        private void InitializeTxtNotes()
        {
            TxtNotes = new PageTextBox(this,
                                       GeneralUtils.GetResourceByName("global_notes"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);

            TxtNotes.Entry.IsEditable = true;
        }

        private void InitializeTxtLocality()
        {
            TxtLocality = new PageTextBox(this,
                                          GeneralUtils.GetResourceByName("global_locality"),
                                          isRequired: false,
                                          isValidatable: false,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true);
        }

        private void InitializeTxtAddress()
        {
            TxtAddress = new PageTextBox(this,
                                         GeneralUtils.GetResourceByName("global_address"),
                                         isRequired: false,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: true);
        }

        private void InitializeTxtDiscount()
        {
            TxtDiscount = new PageTextBox(this,
                                          GeneralUtils.GetResourceByName("global_discount"),
                                          isRequired: true,
                                          isValidatable: true,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true,
                                          regex: RegularExpressions.DecimalNumber);

            TxtDiscount.Text = 0.00M.ToString("");
            TxtDiscount.Entry.Changed += (s, args) => UpdateTotals();
            ValidatableFields.Add(TxtDiscount);
        }

        private void InitializeTxtCardNumber()
        {
            TxtCardNumber = new PageTextBox(this,
                                            GeneralUtils.GetResourceByName("global_card_number"),
                                            isRequired: false,
                                            isValidatable: false,
                                            includeSelectButton: false,
                                            includeKeyBoardButton: true);
        }

        private void InitializeTxtFiscalNumber()
        {
            TxtFiscalNumber = new PageTextBox(this,
                                              GeneralUtils.GetResourceByName("global_fiscal_number"),
                                              isRequired: true,
                                              isValidatable: true,
                                              regex: RegularExpressions.FiscalNumber,
                                              includeSelectButton: false,
                                              includeKeyBoardButton: true);

            ValidatableFields.Add(TxtFiscalNumber);
        }

        private void InitializeTxtCustomer()
        {
            TxtCustomer = new PageTextBox(this,
                                          GeneralUtils.GetResourceByName("global_customer"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: true);

            TxtCustomer.SelectEntityClicked += BtnSelectCustomer_Clicked;
            ValidatableFields.Add(TxtCustomer);
        }

        private void InitializeButtons()
        {
            InitializeScrollersButtons();
            AddEventHandlers();
            BtnFullPayment.Sensitive = false;
        }

        private void InitializeScrollersButtons()
        {
            BtnPrevious = new IconButton(
              new ButtonSettings
              {
                  BackgroundColor = Color.White,
                  Icon = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_subfamily_article_scroll_left.png",
                  IconSize = new Size(62, 31),
                  ButtonSize = AppSettings.Instance.sizePosSmallButtonScroller
              });

            BtnNext = new IconButton(
               new ButtonSettings
               {
                   BackgroundColor = Color.White,
                   Icon = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_subfamily_article_scroll_right.png",
                   IconSize = new Size(62, 31),
                   ButtonSize = AppSettings.Instance.sizePosSmallButtonScroller
               });

            BtnPrevious.Relief = ReliefStyle.None;
            BtnPrevious.BorderWidth = 0;
            BtnPrevious.CanFocus = false;

            BtnNext.Relief = ReliefStyle.None;
            BtnNext.BorderWidth = 0;
            BtnNext.CanFocus = false;
        }
    }
}

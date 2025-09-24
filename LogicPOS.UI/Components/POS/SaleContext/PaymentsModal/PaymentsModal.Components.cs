using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Menus;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System.Collections.Generic;


namespace LogicPOS.UI.Components.POS
{
    public partial class PaymentsModal
    {
       
        private IconButtonWithText BtnOk { get; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnClearCustomer { get; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonClearCustomer_DialogActionArea",
                                                                                                           GeneralUtils.GetResourceByName("global_button_label_payment_dialog_clear_client"),
                                                                                                           AppSettings.Paths.Images + @"Icons\icon_pos_nav_delete.png");
        private IconButtonWithText BtnNewCustomer { get; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonClearCustomer_DialogActionArea",
                                                                                                          GeneralUtils.GetResourceByName("dialog_button_label_new_client"),
                                                                                                          AppSettings.Paths.Images + @"Icons\icon_pos_clients.png");
        private IconButtonWithText BtnFullPayment { get; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonFullPayment_DialogActionArea",
                                                                                                          GeneralUtils.GetResourceByName("global_button_label_payment_dialog_full_payment"),
                                                                                                          AppSettings.Paths.Images + @"Icons\icon_pos_payment_full.png");
        private IconButtonWithText BtnPartialPayment { get; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPartialPayment_DialogActionArea",
                                                                                                            GeneralUtils.GetResourceByName("global_button_label_payment_dialog_partial_payment"),
                                                                                                            AppSettings.Paths.Images + @"Icons\icon_pos_payment_partial.png");
        private IconButtonWithText BtnInvoice { get; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPartialPayment_DialogActionArea",
                                                                                                                   GeneralUtils.GetResourceByName("global_documentfinance_type_title_ft"),
                                                                                                                 AppSettings.Paths.Images + @"Icons\icon_pos_toolbar_finance_document.png");
        private TextBox TxtCustomer { get; set; }
        private TextBox TxtFiscalNumber { get; set; }
        private TextBox TxtCountry { get; set; }
        private TextBox TxtDiscount { get; set; }
        private TextBox TxtCardNumber { get; set; }
        private TextBox TxtAddress { get; set; }
        private TextBox TxtLocality { get; set; }
        private TextBox TxtZipCode { get; set; }
        private TextBox TxtCity { get; set; }
        private TextBox TxtNotes { get; set; }

        public HashSet<IValidatableField> ValidatableFields { get; private set; } = new HashSet<IValidatableField>();

        private Label LabelTotal { get; } = new Label(GeneralUtils.GetResourceByName("global_total_price_to_pay") + ":");
        private Label LabelDelivery { get; } = new Label(GeneralUtils.GetResourceByName("global_total_deliver") + ":");
        private Label LabelChange { get; } = new Label(GeneralUtils.GetResourceByName("global_total_change") + ":");
        private Label LabelTotalValue { get; } = new Label("0") { WidthRequest = 135 };
        private Label LabelDeliveryValue { get; } = new Label("0");
        private Label LabelChangeValue { get; } = new Label("0");

        private IconButtonWithText BtnCurrentAccountMethod { get; set; }
        private IconButtonWithText BtnMoney { get; set; }
        private IconButtonWithText BtnCheck { get; set; }
        private IconButtonWithText BtnMB { get; set; }
        private IconButtonWithText BtnCreditCard { get; set; }
        private IconButtonWithText BtnDebitCard { get; set; }
        private IconButtonWithText BtnVisa { get; set; }
        private IconButtonWithText BtnCustomerCard { get; set; }
        private List<IconButtonWithText> PaymentMethodButtons { get; set; }

    }
}

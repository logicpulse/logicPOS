using Gtk;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Menus;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.POS
{
    public partial class PaymentsModal
    {
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

        private void InitializeButtons()
        {
            BtnFullPayment.Sensitive = false;

            InitializePaymentMethodButtons();

            PaymentMethodButtons = new List<IconButtonWithText> {
                BtnMoney,
                BtnCheck,
                BtnMB,
                BtnCreditCard,
                BtnDebitCard,
                BtnVisa,
                BtnCustomerCard,
                BtnCurrentAccountMethod };

            AddEventHandlers();
        }

        private void InitializeTxtCountry()
        {
            TxtCountry = new TextBox(this,
                                         GeneralUtils.GetResourceByName("global_country"),
                                         isRequired: true,
                                         isValidatable: false,
                                         includeSelectButton: true,
                                         includeKeyBoardButton: false, 
                                         includeClearButton: false);

            TxtCountry.Entry.IsEditable = false;
            TxtCountry.SelectEntityClicked += BtnSelectCountry_Clicked;
            ValidatableFields.Add(TxtCountry);
        }

        private void InitializeTxtCity()
        {
            TxtCity = new TextBox(this,
                                      GeneralUtils.GetResourceByName("global_city"),
                                      isRequired: false,
                                      isValidatable: false,
                                      includeSelectButton: false,
                                      includeKeyBoardButton: true, 
                                      includeClearButton: false);
        }

        private void InitializeTxtZipCode()
        {
            TxtZipCode = new TextBox(this,
                                         GeneralUtils.GetResourceByName("global_zipcode"),
                                         isRequired: false,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: true, 
                                         includeClearButton: false);
        }

        private void InitializeTxtNotes()
        {
            TxtNotes = new TextBox(this,
                                       GeneralUtils.GetResourceByName("global_notes"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true, 
                                       includeClearButton: false);

            TxtNotes.Entry.IsEditable = true;
        }

        private void InitializeTxtLocality()
        {
            TxtLocality = new TextBox(this,
                                          GeneralUtils.GetResourceByName("global_locality"),
                                          isRequired: false,
                                          isValidatable: false,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true, 
                                          includeClearButton: false);
        }

        private void InitializeTxtAddress()
        {
            TxtAddress = new TextBox(this,
                                         GeneralUtils.GetResourceByName("global_address"),
                                         isRequired: false,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: true, 
                                         includeClearButton: false);
        }

        private void InitializeTxtDiscount()
        {
            TxtDiscount = new TextBox(this,
                                          GeneralUtils.GetResourceByName("global_discount"),
                                          isRequired: true,
                                          isValidatable: true,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true,
                                          regex: RegularExpressions.DecimalGreaterEqualThanZeroFinancial,
                                          includeClearButton: false);

            TxtDiscount.Text = 0.00M.ToString("");
            TxtDiscount.Entry.Changed += (s, args) => UpdateTotals();
            ValidatableFields.Add(TxtDiscount);
        }

        private void InitializeTxtCardNumber()
        {
            TxtCardNumber = new TextBox(this,
                                            GeneralUtils.GetResourceByName("global_card_number"),
                                            isRequired: false,
                                            isValidatable: false,
                                            includeSelectButton: false,
                                            includeKeyBoardButton: true,
                                          includeClearButton: false);
        }

        private void InitializeTxtFiscalNumber()
        {
            TxtFiscalNumber = new TextBox(this,
                                              GeneralUtils.GetResourceByName("global_fiscal_number"),
                                              isRequired: true,
                                              isValidatable: true,
                                              regex: RegularExpressions.FiscalNumber,
                                              includeSelectButton: false,
                                              includeKeyBoardButton: true,
                                              includeClearButton: false);

            ValidatableFields.Add(TxtFiscalNumber);
            var customers = CustomersService.Customers.Select(c => (c as object, c.FiscalNumber)).ToList();
            TxtFiscalNumber.WithAutoCompletion(customers);
            TxtFiscalNumber.OnCompletionSelected += c => SelectCustomer(c as Customer);
            TxtFiscalNumber.Entry.Changed += TxtFiscalNumber_Changed;
        }

        private void InitializeTxtCustomer()
        {
            TxtCustomer = new TextBox(this,
                                          GeneralUtils.GetResourceByName("global_customer"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: true,
                                          includeClearButton: false);


            ValidatableFields.Add(TxtCustomer);

            TxtCustomer.SelectEntityClicked += BtnSelectCustomer_Clicked;
            var customers = CustomersService.Customers.Select(c => (c as object, c.Name)).ToList();
            TxtCustomer.WithAutoCompletion(customers);
            TxtCustomer.OnCompletionSelected += c => SelectCustomer(c as Customer);
            TxtCustomer.Entry.Changed += TxtCustomer_Changed;
        }

        private void InitializePaymentMethodButtons()
        {
            BtnMoney = CreatePaymentMethodButton(GeneralUtils.GetResourceByName("pos_button_label_payment_type_money"),
                                                             AppSettings.Paths.Images + @"Icons/icon_pos_payment_type_money.png");

            BtnCheck = CreatePaymentMethodButton(GeneralUtils.GetResourceByName("pos_button_label_payment_type_bank_check"),
                                                  AppSettings.Paths.Images + @"Icons/icon_pos_payment_type_bank_check.png");

            BtnMB = CreatePaymentMethodButton(GeneralUtils.GetResourceByName("pos_button_label_payment_type_cash_machine"),
                                              AppSettings.Paths.Images + @"Icons/icon_pos_payment_type_cash_machine.png");

            BtnCreditCard = CreatePaymentMethodButton(GeneralUtils.GetResourceByName("pos_button_label_payment_type_credit_card"),
                                                      AppSettings.Paths.Images + @"Icons/icon_pos_payment_type_credit_card.png");

            BtnDebitCard = CreatePaymentMethodButton(GeneralUtils.GetResourceByName("pos_button_label_payment_type_debit_card"),
                                                     AppSettings.Paths.Images + @"Icons/icon_pos_payment_type_debit_card.png");

            BtnVisa = CreatePaymentMethodButton(GeneralUtils.GetResourceByName("pos_button_label_payment_type_visa"),
                                                AppSettings.Paths.Images + @"Icons/icon_pos_payment_type_visa.png");

            BtnCustomerCard = CreatePaymentMethodButton(GeneralUtils.GetResourceByName("pos_button_label_payment_type_customer_card"),
                                                        AppSettings.Paths.Images + @"Icons/icon_pos_payment_type_customer_card.png");

            BtnCurrentAccountMethod = CreatePaymentMethodButton(GeneralUtils.GetResourceByName("pos_button_label_payment_type_current_account"),
                                                          AppSettings.Paths.Images + @"Icons/icon_pos_payment_type_current_account.png");
        }

    }
}

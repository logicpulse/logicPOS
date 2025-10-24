using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PayInvoiceModal
    {
        private void Initialize()
        {
            InitializeTxtPaymentMethod();
            InitializeTxtCurrency();
            InitializeTxtExchangeRate();
            InitializeTxtTotalPaid();
            InitializeTxtSystemCurrencyTotalPaid();
            InitializeTxtDateTime();
            InitializeTxtNotes();

            ValidatableFields.Add(TxtPaymentMethod);
            ValidatableFields.Add(TxtCurrency);
            ValidatableFields.Add(TxtTotalPaid);
            ValidatableFields.Add(TxtDateTime);

            AddEventHandlers();
        }
      
        private void InitializeTxtSystemCurrencyTotalPaid()
        {
            TxtSystemCurrencyTotalPaid = new TextBox(this,
                                               "",
                                               isRequired: false,
                                               isValidatable: false,
                                               includeSelectButton: false,
                                               includeKeyBoardButton: false);

            TxtSystemCurrencyTotalPaid.Entry.Sensitive = false;
        }

        private void InitializeTxtExchangeRate()
        {
            TxtExchangeRate = new TextBox(this,
                                              GeneralUtils.GetResourceByName("global_exchangerate"),
                                              isRequired: true,
                                              isValidatable: true,
                                              regex: RegularExpressions.DecimalNumber,
                                              includeSelectButton: false,
                                              includeKeyBoardButton: true);

            TxtExchangeRate.Text = "1";

            TxtExchangeRate.Entry.Changed += TxtExchangeRate_Changed;
        }

        private void InitializeTxtNotes()
        {
            TxtNotes = new TextBox(this,
                                       GeneralUtils.GetResourceByName("global_notes"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);
        }

        private void InitializeTxtDateTime()
        {
            TxtDateTime = new TextBox(this,
                                          GeneralUtils.GetResourceByName("global_date"),
                                          isRequired: true,
                                          isValidatable: true,
                                          regex: RegularExpressions.DateTime,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true);

            TxtDateTime.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        private void InitializeTxtTotalPaid()
        {
            TxtTotalPaid = new TextBox(this,
                                          GeneralUtils.GetResourceByName("global_total_deliver"),
                                          isRequired: true,
                                          isValidatable: true,
                                          regex: RegularExpressions.Money,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true);

            TxtTotalPaid.Entry.Changed += TxtTotalPaid_Changed;
        }
        
        private void InitializeTxtPaymentMethod()
        {
            TxtPaymentMethod = new TextBox(this,
                                               GeneralUtils.GetResourceByName("global_payment_method"),
                                               isRequired: true,
                                               isValidatable: false,
                                               includeSelectButton: true,
                                               includeKeyBoardButton: false);

            TxtPaymentMethod.Entry.IsEditable = false;

            TxtPaymentMethod.SelectEntityClicked += BtnSelectPaymentMethod_Clicked;
        }
        
        private void InitializeTxtCurrency()
        {
            TxtCurrency = new TextBox(this,
                                          GeneralUtils.GetResourceByName("global_currency"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false);

            TxtCurrency.Entry.IsEditable = false;

            TxtCurrency.SelectEntityClicked += BtnSelectCurrency_Clicked;
        }
    }
}

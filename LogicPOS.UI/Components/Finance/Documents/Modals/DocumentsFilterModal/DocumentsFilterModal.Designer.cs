using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.Finance.DocumentTypes;
using LogicPOS.UI.Components.Finance.PaymentConditions;
using LogicPOS.UI.Components.Finance.PaymentMethods;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Documents
{
    public partial class DocumentsFilterModal
    {
        private void InitializeTextBoxes()
        {
            InitializeTxtStartDate();
            InitializeTxtEndDate();
            InitializeTxtDocumentType();
            InitializeTxtCustomer();
            InitializeTxtPaymentMethod();
            InitializeTxtPaymentCondition();
            InitializeComboPaymentStatus();
        }
       
        private void InitializeComboPaymentStatus()
        {
            ComboPaymentStatus = new PageComboBox<int>("Estado do Pagamento");
            ComboPaymentStatus.AddItem(0, "Todos");
            ComboPaymentStatus.AddItem(1, "Pago");
            ComboPaymentStatus.AddItem(2, "Não Pago");
        }
       
        private void InitializeTxtCustomer()
        {
            TxtCustomer = new TextBox(this,
                                          GeneralUtils.GetResourceByName("global_customer"),
                                          isRequired: false,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false);

            TxtCustomer.Entry.IsEditable = true;
            TxtCustomer.WithAutoCompletion(CustomersService.AutocompleteLines,id => CustomersService.GetById(id));
            TxtCustomer.OnCompletionSelected += c => SelectCustomer(c as Customer);
            TxtCustomer.Entry.Changed += TxtCustomer_Changed;
            TxtCustomer.SelectEntityClicked += BtnSelectCustomer_Clicked;
        }
        
        private void InitializeTxtDocumentType()
        {
            TxtDocumentType = new TextBox(this,
                                              GeneralUtils.GetResourceByName("global_documentfinanceseries_documenttype"),
                                              isRequired: false,
                                              isValidatable: false,
                                              includeSelectButton: true,
                                              includeKeyBoardButton: false);

            TxtDocumentType.Entry.IsEditable = true;
            TxtDocumentType.WithAutoCompletion(DocumentTypesService.AutocompleteLines, id => DocumentTypesService.GetById(id));
            TxtDocumentType.OnCompletionSelected += d => SelectDocumentType(d as DocumentType);
            TxtDocumentType.Entry.Changed += TxtDocumentType_Changed;
            TxtDocumentType.SelectEntityClicked += BtnSelectDocumentType_Clicked;
        }
        
        private void InitializeTxtPaymentCondition()
        {
            TxtPaymentCondition = new TextBox(this,
                                                   GeneralUtils.GetResourceByName("global_payment_condition"),
                                                   isRequired: false,
                                                   isValidatable: false,
                                                   includeSelectButton: true,
                                                   includeKeyBoardButton: false);

            TxtPaymentCondition.Entry.IsEditable = true;
            TxtPaymentCondition.WithAutoCompletion(PaymentConditionsService.AutocompleteLines, id => PaymentConditionsService.GetById(id));
            TxtPaymentCondition.OnCompletionSelected += p => SelectPaymentCondition(p as PaymentCondition);
            TxtPaymentCondition.Entry.Changed += TxtPaymentCondition_Changed;
            TxtPaymentCondition.SelectEntityClicked += BtnSelectPaymentCondition_Clicked;
        }
       
        private void InitializeTxtPaymentMethod()
        {
            TxtPaymentMethod = new TextBox(this,
                                               GeneralUtils.GetResourceByName("global_payment_method"),
                                               isRequired: false,
                                               isValidatable: false,
                                               includeSelectButton: true,
                                               includeKeyBoardButton: false);

            TxtPaymentMethod.Entry.IsEditable = true;
            TxtPaymentMethod.WithAutoCompletion(PaymentMethodsService.AutocompleteLines, id => PaymentMethodsService.GetBydId(id));
            TxtPaymentMethod.OnCompletionSelected += p => SelectPaymentMethod(p as PaymentMethod);
            TxtPaymentMethod.Entry.Changed += TxtPaymentMethod_Changed;
            TxtPaymentMethod.SelectEntityClicked += BtnSelectPaymentMethod_Clicked;
        }
       
        private void InitializeTxtStartDate()
        {
            TxtStartDate = new TextBox(this,
                                           GeneralUtils.GetResourceByName("global_date_start"),
                                           isRequired: false,
                                           isValidatable: true,
                                           regex: RegularExpressions.Date,
                                           includeSelectButton: true,
                                           includeKeyBoardButton: true);

            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            TxtStartDate.Text = firstDayOfMonth.ToString("yyyy-MM-dd");
            TxtStartDate.Entry.Changed += TxtStartDate_Entry_Changed;
            TxtStartDate.SelectEntityClicked += TxtStartDate_SelectEntityClicked;
        }
       
        private void InitializeTxtEndDate()
        {
            TxtEndDate = new TextBox(this,
                                           GeneralUtils.GetResourceByName("global_date_end"),
                                           isRequired: false,
                                           isValidatable: true,
                                           regex: RegularExpressions.Date,
                                           includeSelectButton: true,
                                           includeKeyBoardButton: true);

            TxtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            TxtEndDate.Entry.Changed += TxtEndDate_Entry_Changed;
            TxtEndDate.SelectEntityClicked += TxtEndDate_SelectEntityClicked;
        }

        private void TxtStartDate_Entry_Changed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtStartDate.Text) && TxtStartDate.Text.Length >= 10)
            {
                if (TxtStartDate.IsValid())
                {
                    TxtStartDate.Text = TxtStartDate.Text.ValidateDate();
                }
                return;
            }
        }

        private void TxtEndDate_Entry_Changed(object sender, EventArgs e)
        {
            if ((!string.IsNullOrEmpty(TxtEndDate.Text)) && TxtEndDate.Text.Length >= 10)
            {
                if (TxtEndDate.IsValid())
                {
                    TxtEndDate.Text = TxtEndDate.Text.ValidateDate();
                }
                return;
            }
        }
    }
}

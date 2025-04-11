using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
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

            TxtCustomer.Entry.IsEditable = false;

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

            TxtDocumentType.Entry.IsEditable = false;

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

            TxtPaymentCondition.Entry.IsEditable = false;

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

            TxtPaymentMethod.Entry.IsEditable = false;

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

            TxtEndDate.SelectEntityClicked += TxtEndDate_SelectEntityClicked;
        }

    }
}

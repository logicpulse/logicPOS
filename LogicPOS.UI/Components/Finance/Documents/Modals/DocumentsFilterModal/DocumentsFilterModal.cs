using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.GetDocuments;
using LogicPOS.Api.Features.Receipts.GetReceipts;
using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.Finance.DocumentTypes;
using LogicPOS.UI.Components.Finance.PaymentConditions;
using LogicPOS.UI.Components.Finance.PaymentMethods;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Documents
{
    public partial class DocumentsFilterModal : Modal
    {
        public DocumentsFilterModal(Window parent) :
            base(parent,
                LocalizedString.Instance["window_title_dialog_filter"],
                new Size(540, 568),
                AppSettings.Paths.Images + @"Icons\Windows\icon_window_date_picker.png")
        {
            WindowSettings.Close.Hide();
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            AddEventHandlers();

            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnClear, ResponseType.None),
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };
        }

        public bool AllFieldsAreValid() => GetValidatableFields().All(field => field.IsValid());

        private IEnumerable<IValidatableField> GetValidatableFields()
        {
            return new List<IValidatableField>
            {
                TxtStartDate,
                TxtEndDate
            };
        }

        private void ShowValidationErrors() => ValidationUtilities.ShowValidationErrors(GetValidatableFields(), this);

        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
            BtnClear.Clicked += BtnClear_Clicked;
        }

        public GetDocumentsQuery GetDocumentsQuery()
        {
            if(AllFieldsAreValid() == false)
            {
                return null;
            }

            var query = new GetDocumentsQuery();

            if(string.IsNullOrWhiteSpace(TxtStartDate.Text) == false)
            {
                query.StartDate = DateTime.Parse(TxtStartDate.Text);
            }

            if (string.IsNullOrWhiteSpace(TxtEndDate.Text) == false)
            {
                query.EndDate = DateTime.Parse(TxtEndDate.Text);
            }

            if (TxtDocumentType.SelectedEntity != null)
            {
                var documentType = TxtDocumentType.SelectedEntity as DocumentType;
                query.Types = new string[] { documentType.Acronym };
            }

            if (TxtCustomer.SelectedEntity != null)
            {
                var customer = TxtCustomer.SelectedEntity as Customer;
                query.CustomerId = customer.Id;
            }

            if (TxtPaymentMethod.SelectedEntity != null)
            {
                var paymentMethod = TxtPaymentMethod.SelectedEntity as PaymentMethod;
                query.PaymentMethodId = paymentMethod.Id;
            }

            if (TxtPaymentCondition.SelectedEntity != null)
            {
                var paymentCondition = TxtPaymentCondition.SelectedEntity as PaymentCondition;
                query.PaymentConditionId = paymentCondition.Id;
            }

            int paymentStatus = ComboPaymentStatus.GetSelectedItem();
            if (paymentStatus != 0)
            {
                query.PaymentStatus = (DocumentPaymentStatusFilter)ComboPaymentStatus.GetSelectedItem();
            }

            return query;
        }

        public GetReceiptsQuery GetReceiptsQuery()
        {
            if (AllFieldsAreValid() == false)
            {
                return null;
            }

            var query = new GetReceiptsQuery();

            if (string.IsNullOrWhiteSpace(TxtStartDate.Text) == false)
            {
                query.StartDate = DateTime.Parse(TxtStartDate.Text);
            }

            if (string.IsNullOrWhiteSpace(TxtEndDate.Text) == false)
            {
                query.EndDate = DateTime.Parse(TxtEndDate.Text);
            }

            if (TxtCustomer.SelectedEntity != null)
            {
                var customer = TxtCustomer.SelectedEntity as Customer;
                query.CustomerId = customer.Id;
            }

            if (TxtPaymentMethod.SelectedEntity != null)
            {
                var paymentMethod = TxtPaymentMethod.SelectedEntity as PaymentMethod;
                query.PaymentMethodId = paymentMethod.Id;
            }

            return query;
        }
        protected override void OnResponse(ResponseType response)
        {
            if (response == ResponseType.None)
            {
                Run();
                return;
            }
        }

        protected override Widget CreateBody()
        {
            var verticalLayout = new VBox(false, 2);

            InitializeTextBoxes();
            verticalLayout.PackStart(TextBox.CreateHbox(TxtStartDate, TxtEndDate), false, false, 0);
            verticalLayout.PackStart(TxtDocumentType.Component, false, false, 0);
            verticalLayout.PackStart(TxtCustomer.Component, false, false, 0);
            verticalLayout.PackStart(TxtPaymentMethod.Component, false, false, 0);
            verticalLayout.PackStart(TxtPaymentCondition.Component, false, false, 0);
            verticalLayout.PackStart(ComboPaymentStatus.Component, false, false, 0);

            return verticalLayout;
        }

        private void SelectCustomer(Customer customer)
        {
            TxtCustomer.SelectedEntity = customer;
            TxtCustomer.Text = customer.Name;
        }

        private void SelectDocumentType(DocumentType documentType)
        {
            TxtDocumentType.SelectedEntity = documentType;
            TxtDocumentType.Text = documentType.Designation;
        }

        private void SelectPaymentMethod(PaymentMethod paymentMethod)
        {
            TxtPaymentMethod.SelectedEntity = paymentMethod;
            TxtPaymentMethod.Text = paymentMethod.Designation;
        }

        private void SelectPaymentCondition(PaymentCondition paymentCondition)
        {
            TxtPaymentCondition.SelectedEntity = paymentCondition;
            TxtPaymentCondition.Text = paymentCondition.Designation;
        }
    }
}

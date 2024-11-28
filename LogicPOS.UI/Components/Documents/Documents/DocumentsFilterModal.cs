using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Entities.Enums;
using LogicPOS.Api.Features.Documents.GetDocuments;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Documents
{
    public class DocumentsFilterModal : Modal
    {
        #region Components
        private PageTextBox TxtStartDate { get; set; }
        private PageTextBox TxtEndDate { get; set; }
        private PageTextBox TxtDocumentType { get; set; }
        private PageTextBox TxtCustomer { get; set; }
        private PageTextBox TxtPaymentMethod { get; set; }
        private PageTextBox TxtPaymentCondition { get; set; }
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnClear { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.CleanFilter);
        #endregion

        public DocumentsFilterModal(Window parent) :
            base(parent,
                LocalizedString.Instance["window_title_dialog_filter"],
                new Size(540, 568),
                PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_date_picker.png")
        {
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

        public GetDocumentsQuery GetQuery()
        {
            if(AllFieldsAreValid() == false)
            {
                return null;
            }

            var query = new GetDocumentsQuery
            {
                StartDate = DateTime.Parse(TxtStartDate.Text),
                EndDate = DateTime.Parse(TxtEndDate.Text)
            };

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

            return query;
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            if (!AllFieldsAreValid())
            {
                ShowValidationErrors();
                Run();
                return;
            }
        }

        private void BtnClear_Clicked(object sender, EventArgs e)
        {
            TxtStartDate.Clear();
            TxtEndDate.Clear();
            TxtDocumentType.Clear();
            TxtCustomer.Clear();
            TxtPaymentMethod.Clear();
            TxtPaymentCondition.Clear();
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
            var vbox = new VBox(false, 2);

            InitializeTextBoxes();

            vbox.PackStart(TxtStartDate.Component, false, false, 0);
            vbox.PackStart(TxtEndDate.Component, false, false, 0);
            vbox.PackStart(TxtDocumentType.Component, false, false, 0);
            vbox.PackStart(TxtCustomer.Component, false, false, 0);
            vbox.PackStart(TxtPaymentMethod.Component, false, false, 0);
            vbox.PackStart(TxtPaymentCondition.Component, false, false, 0);

            return vbox;
        }

        private void InitializeTextBoxes()
        {
            InitializeTxtStartDate();
            InitializeTxtEndDate();
            InitializeTxtDocumentType();
            InitializeTxtCustomer();
            InitializeTxtPaymentMethod();
            InitializeTxtPaymentCondition();
        }

        private void InitializeTxtCustomer()
        {
            TxtCustomer = new PageTextBox(this,
                                          GeneralUtils.GetResourceByName("global_customer"),
                                          isRequired: false,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false);

            TxtCustomer.Entry.IsEditable = false;

            TxtCustomer.SelectEntityClicked += BtnSelectCustomer_Clicked;
        }

        private void BtnSelectCustomer_Clicked(object sender, System.EventArgs e)
        {
            var page = new CustomersPage(null, PageOptions.SelectionPageOptions);
            var selectDocumentTypeModal = new EntitySelectionModal<Customer>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectDocumentTypeModal.Run();
            selectDocumentTypeModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtCustomer.Text = page.SelectedEntity.Name;
                TxtCustomer.SelectedEntity = page.SelectedEntity;
            }
        }

        private void InitializeTxtDocumentType()
        {
            TxtDocumentType = new PageTextBox(this,
                                              GeneralUtils.GetResourceByName("global_documentfinanceseries_documenttype"),
                                              isRequired: false,
                                              isValidatable: false,
                                              includeSelectButton: true,
                                              includeKeyBoardButton: false);

            TxtDocumentType.Entry.IsEditable = false;

            TxtDocumentType.SelectEntityClicked += BtnSelectDocumentType_Clicked;
        }

        private void BtnSelectDocumentType_Clicked(object sender, System.EventArgs e)
        {
            var page = new DocumentTypesPage(null, PageOptions.SelectionPageOptions);
            var selectDocumentTypeModal = new EntitySelectionModal<DocumentType>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectDocumentTypeModal.Run();
            selectDocumentTypeModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtDocumentType.Text = page.SelectedEntity.Designation;
                TxtDocumentType.SelectedEntity = page.SelectedEntity;
            }
        }

        private void InitializeTxtPaymentCondition()
        {
            TxtPaymentCondition = new PageTextBox(this,
                                                   GeneralUtils.GetResourceByName("global_payment_condition"),
                                                   isRequired: false,
                                                   isValidatable: false,
                                                   includeSelectButton: true,
                                                   includeKeyBoardButton: false);

            TxtPaymentCondition.Entry.IsEditable = false;

            TxtPaymentCondition.SelectEntityClicked += BtnSelectPaymentCondition_Clicked;
        }

        private void BtnSelectPaymentCondition_Clicked(object sender, EventArgs e)
        {
            var page = new PaymentConditionsPage(null, PageOptions.SelectionPageOptions);
            var selectPaymentConditionModal = new EntitySelectionModal<PaymentCondition>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectPaymentConditionModal.Run();
            selectPaymentConditionModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtPaymentCondition.Text = page.SelectedEntity.Designation;
                TxtPaymentCondition.SelectedEntity = page.SelectedEntity;
            }
        }

        private void InitializeTxtPaymentMethod()
        {
            TxtPaymentMethod = new PageTextBox(this,
                                               GeneralUtils.GetResourceByName("global_payment_method"),
                                               isRequired: false,
                                               isValidatable: false,
                                               includeSelectButton: true,
                                               includeKeyBoardButton: false);

            TxtPaymentMethod.Entry.IsEditable = false;

            TxtPaymentMethod.SelectEntityClicked += BtnSelectPaymentMethod_Clicked;
        }

        private void BtnSelectPaymentMethod_Clicked(object sender, EventArgs e)
        {
            var page = new PaymentMethodsPage(null, PageOptions.SelectionPageOptions);
            var selectPaymentMethodModal = new EntitySelectionModal<PaymentMethod>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectPaymentMethodModal.Run();
            selectPaymentMethodModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtPaymentMethod.Text = page.SelectedEntity.Designation;
                TxtPaymentMethod.SelectedEntity = page.SelectedEntity;
            }
        }

        private void InitializeTxtStartDate()
        {
            TxtStartDate = new PageTextBox(this,
                                           GeneralUtils.GetResourceByName("global_date_start"),
                                           isRequired: true,
                                           isValidatable: true,
                                           regex: RegularExpressions.Date,
                                           includeSelectButton: true,
                                           includeKeyBoardButton: true);

            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            TxtStartDate.Text = firstDayOfMonth.ToString("yyyy-MM-dd");

            TxtStartDate.SelectEntityClicked += TxtStartDate_SelectEntityClicked;
        }

        private void TxtStartDate_SelectEntityClicked(object sender, EventArgs e)
        {
            var dateTimePicker = new DateTimePicker(this);
            ResponseType response = (ResponseType)dateTimePicker.Run();

            if (response == ResponseType.Ok)
            {
                TxtStartDate.Text = dateTimePicker.Calendar.Date.ToString("yyyy-MM-dd");
            }

            dateTimePicker.Destroy();
        }

        private void InitializeTxtEndDate()
        {
            TxtEndDate = new PageTextBox(this,
                                           GeneralUtils.GetResourceByName("global_date_end"),
                                           isRequired: true,
                                           isValidatable: true,
                                           regex: RegularExpressions.Date,
                                           includeSelectButton: true,
                                           includeKeyBoardButton: true);

            TxtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            TxtEndDate.SelectEntityClicked += TxtEndDate_SelectEntityClicked;
        }

        private void TxtEndDate_SelectEntityClicked(object sender, EventArgs e)
        {
            var dateTimePicker = new DateTimePicker(this);
            ResponseType response = (ResponseType)dateTimePicker.Run();

            if (response == ResponseType.Ok)
            {
                TxtEndDate.Text = dateTimePicker.Calendar.Date.ToString("yyyy-MM-dd");
            }

            dateTimePicker.Destroy();
        }
    }
}

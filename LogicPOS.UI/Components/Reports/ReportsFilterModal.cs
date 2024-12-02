using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public class ReportsFilterModal : Modal
    {
        #region Components
        private PageTextBox TxtStartDate { get; set; }
        private PageTextBox TxtEndDate { get; set; }
        private PageTextBox TxtDocumentType { get; set; }
        private PageTextBox TxtCustomer { get; set; }
        private PageTextBox TxtWarehouse { get; set; }
        private PageTextBox TxtVatRate { get; set; }
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnClear { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.CleanFilter);
        #endregion

        public ReportsFilterModal(Window parent) :
          base(parent,
              LocalizedString.Instance["window_title_dialog_report_filter"],
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
            TxtWarehouse.Clear();
            TxtVatRate.Clear();
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
            vbox.PackStart(TxtWarehouse.Component, false, false, 0);
            vbox.PackStart(TxtVatRate.Component, false, false, 0);

            return vbox;
        }

        private void InitializeTextBoxes()
        {
            InitializeTxtStartDate();
            InitializeTxtEndDate();
            InitializeTxtDocumentType();
            InitializeTxtCustomer();
            InitializeTxtWarehouse();
            InitializeTxtVatRate();
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

        private void InitializeTxtVatRate()
        {
            TxtVatRate = new PageTextBox(this,
                                         GeneralUtils.GetResourceByName("global_vat_rates"),
                                         isRequired: false,
                                         isValidatable: false,
                                         includeSelectButton: true,
                                         includeKeyBoardButton: false);

            TxtVatRate.Entry.IsEditable = false;

            TxtVatRate.SelectEntityClicked += BtnSelectVatRate_Clicked;
        }

        private void BtnSelectVatRate_Clicked(object sender, EventArgs e)
        {
            var page = new VatRatesPage(null, PageOptions.SelectionPageOptions);
            var selectVatRateModal = new EntitySelectionModal<VatRate>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectVatRateModal.Run();
            selectVatRateModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtVatRate.Text = page.SelectedEntity.Designation;
                TxtVatRate.SelectedEntity = page.SelectedEntity;
            }
        }

        private void InitializeTxtWarehouse()
        {
            TxtWarehouse = new PageTextBox(this,
                                           GeneralUtils.GetResourceByName("global_warehouse"),
                                           isRequired: false,
                                           isValidatable: false,
                                           includeSelectButton: true,
                                           includeKeyBoardButton: false);

            TxtWarehouse.Entry.IsEditable = false;

            TxtWarehouse.SelectEntityClicked += BtnSelectWarehouse_Clicked;
        }

        private void BtnSelectWarehouse_Clicked(object sender, EventArgs e)
        {
            var page = new WarehousesPage(null, PageOptions.SelectionPageOptions);
            var selectWarehouseModal = new EntitySelectionModal<Warehouse>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectWarehouseModal.Run();
            selectWarehouseModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtWarehouse.Text = page.SelectedEntity.Designation;
                TxtWarehouse.SelectedEntity = page.SelectedEntity;
            }
        }

        private void InitializeTxtStartDate()
        {
            TxtStartDate = new PageTextBox(this,
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
                                           isRequired: false,
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

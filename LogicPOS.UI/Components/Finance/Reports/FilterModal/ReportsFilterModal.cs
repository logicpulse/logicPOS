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
    public partial class ReportsFilterModal : Modal
    {
        #region Components
        public TextBox TxtStartDate { get; set; }
        public TextBox TxtEndDate { get; set; }
        public TextBox TxtDocumentType { get; set; }
        public TextBox TxtCustomer { get; set; }
        public TextBox TxtWarehouse { get; set; }
        public TextBox TxtVatRate { get; set; }
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

        private void InitializeTxtVatRate()
        {
            TxtVatRate = new TextBox(this,
                                         GeneralUtils.GetResourceByName("global_vat_rates"),
                                         isRequired: false,
                                         isValidatable: false,
                                         includeSelectButton: true,
                                         includeKeyBoardButton: false);

            TxtVatRate.Entry.IsEditable = false;

            TxtVatRate.SelectEntityClicked += BtnSelectVatRate_Clicked;
        }

        private void InitializeTxtWarehouse()
        {
            TxtWarehouse = new TextBox(this,
                                           GeneralUtils.GetResourceByName("global_warehouse"),
                                           isRequired: false,
                                           isValidatable: false,
                                           includeSelectButton: true,
                                           includeKeyBoardButton: false);

            TxtWarehouse.Entry.IsEditable = false;

            TxtWarehouse.SelectEntityClicked += BtnSelectWarehouse_Clicked;
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

        public DateTime StartDate =>  DateTime.ParseExact(TxtStartDate.Text,"yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture);
        public DateTime EndDate => DateTime.ParseExact(TxtEndDate.Text, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
    }
}

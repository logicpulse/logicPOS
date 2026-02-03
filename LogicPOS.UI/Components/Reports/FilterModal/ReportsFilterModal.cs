using Gtk;
using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ReportsFilterModal : Modal
    {
        public ReportsFilterModal(Window parent) :
          base(parent,
              LocalizedString.Instance["window_title_dialog_report_filter"],
              new Size(500, 509),
              AppSettings.Paths.Images + @"Icons\Windows\icon_window_date_picker.png")
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
            vbox.PackStart(TxtArticle.Component, false, false, 0);
            vbox.PackStart(TxtSerialNumber.Component, false, false, 0);
            vbox.PackStart(TxtDocumentNumber.Component, false, false, 0);
            vbox.PackStart(TxtSubfamily.Component, false, false, 0);
            vbox.PackStart(TxtFamily.Component, false, false, 0);

            return vbox;
        }

        public DateTime StartDate => DateTime.ParseExact(TxtStartDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        public DateTime EndDate => DateTime.ParseExact(TxtEndDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);


    }
}

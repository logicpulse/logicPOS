using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class HolidayModal
    {
        public override Size ModalSize => new Size(500, 500);
        public override string ModalTitleResourceName => "global_holidays";

        #region Components
        private TextBox _txtOrder = TextBoxes.CreateOrderField();
        private TextBox _txtCode = TextBoxes.CreateCodeField();
        private TextBox _txtDesignation = TextBoxes.CreateDesignationField();
        private TextBox _txtDescription= new TextBox("global_description");
        private TextBox _txtDay = new TextBox("global_day", true, true, "^(0?[1-9]|[12][0-9]|3[01])$");
        private TextBox _txtMonth = new TextBox("global_month", true, true, "^(0?[1-9]|1[0-2])$");
        private TextBox _txtYear = new TextBox("global_year", true, true, "^[0-9]+$");
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
        #endregion

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtDesignation.Entry);
            SensitiveFields.Add(_txtDescription.Entry);
            SensitiveFields.Add(_txtDay.Entry);
            SensitiveFields.Add(_txtMonth.Entry);
            SensitiveFields.Add(_txtYear.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkDisabled);
        }

        protected override void AddValidatableFields()
        {

            switch (_modalMode)
            {
                case EntityModalMode.Insert:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtDay);
                    ValidatableFields.Add(_txtMonth);
                    ValidatableFields.Add(_txtYear);
                    break;
                case EntityModalMode.Update:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtOrder);
                    ValidatableFields.Add(_txtCode);
                    ValidatableFields.Add(_txtDay);
                    ValidatableFields.Add(_txtMonth);
                    break;
            }
        }

        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            yield return (CreateDetailsTab(), GeneralUtils.GetResourceByName("global_record_main_detail"));
            yield return (CreateNotesTab(), GeneralUtils.GetResourceByName("global_notes"));
        }

        private VBox CreateDetailsTab()
        {
            var tab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            if (_modalMode != EntityModalMode.Insert)
            {
                tab1.PackStart(_txtOrder.Component, false, false, 0);
                tab1.PackStart(_txtCode.Component, false, false, 0);
            }

            tab1.PackStart(_txtDesignation.Component, false, false, 0);
            tab1.PackStart(_txtDescription.Component, false, false, 0);
            tab1.PackStart(_txtDay.Component, false, false, 0);
            tab1.PackStart(_txtMonth.Component, false, false, 0);
            tab1.PackStart(_txtYear.Component, false, false, 0);

            if (_modalMode != EntityModalMode.Insert)
            {
                tab1.PackStart(_checkDisabled, false, false, 0);
            }

            return tab1;
        }
    }
}

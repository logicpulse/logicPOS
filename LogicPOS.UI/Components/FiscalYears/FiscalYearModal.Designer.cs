using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class FiscalYearModal
    {
        public override Size ModalSize => new Size(500, 550);
        public override string ModalTitleResourceName => "global_fiscal_year";

        #region Components
        private TextBox _txtOrder = TextBoxes.CreateOrderField();
        private TextBox _txtCode = TextBoxes.CreateCodeField();
        private TextBox _txtDesignation = TextBoxes.CreateDesignationField();
        private TextBox _txtYear = new TextBox("global_fiscal_year", true, true, @"^\d+$");
        private TextBox _txtAcronym = new TextBox("global_acronym", true, true, @"^[a-zA-Z0-9]+$");
        private CheckButton _checkSeriesForEachTerminal = new CheckButton(GeneralUtils.GetResourceByName("global_seriesforeachterminal"));
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
        #endregion

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtDesignation.Entry);
            SensitiveFields.Add(_txtYear.Entry);
            SensitiveFields.Add(_txtAcronym.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkDisabled);
        }

        protected override void AddValidatableFields()
        {

            switch (_modalMode)
            {
                case EntityEditionModalMode.Insert:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtYear);
                    ValidatableFields.Add(_txtAcronym);

                    break;
                case EntityEditionModalMode.Update:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtYear);
                    ValidatableFields.Add(_txtAcronym);
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

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                tab1.PackStart(_txtOrder.Component, false, false, 0);
                tab1.PackStart(_txtCode.Component, false, false, 0);

            }

            tab1.PackStart(_txtDesignation.Component, false, false, 0);
            tab1.PackStart(_txtYear.Component, false, false, 0);
            tab1.PackStart(_txtAcronym.Component, false, false, 0);
            tab1.PackStart(_checkSeriesForEachTerminal, false, false, 0);

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                tab1.PackStart(_checkDisabled, false, false, 0);
            }

            return tab1;
        }
    }
}

using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class VatRateModal
    {
        public override Size ModalSize => new Size(500, 550);
        public override string ModalTitleResourceName => "dialog_edit_configurationvatrate_tab1_label";

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtDesignation.Entry);
            SensitiveFields.Add(_txtValue.Entry);
            SensitiveFields.Add(_txtTaxType.Entry);
            SensitiveFields.Add(_txtTaxCode.Entry);
            SensitiveFields.Add(_txtCountryRegionCode.Entry);
            SensitiveFields.Add(_txtDescription.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkDisabled);
        }

        protected override void AddValidatableFields()
        {

            switch (_modalMode)
            {
                case EntityEditionModalMode.Insert:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtValue);
                    ValidatableFields.Add(_txtTaxType);
                    ValidatableFields.Add(_txtTaxCode);
                    ValidatableFields.Add(_txtCountryRegionCode);
                    ValidatableFields.Add(_txtDescription);


                    break;
                case EntityEditionModalMode.Update:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtValue);
                    ValidatableFields.Add(_txtTaxType);
                    ValidatableFields.Add(_txtTaxCode);
                    ValidatableFields.Add(_txtCountryRegionCode);
                    ValidatableFields.Add(_txtDescription);
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
            tab1.PackStart(_txtValue.Component, false, false, 0);
            tab1.PackStart(_txtTaxType.Component, false, false, 0);
            tab1.PackStart(_txtTaxCode.Component, false, false, 0);
            tab1.PackStart(_txtCountryRegionCode.Component, false, false, 0);
            tab1.PackStart(_txtDescription.Component, false, false, 0);

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                tab1.PackStart(_checkDisabled, false, false, 0);
            }

            return tab1;
        }
    }
}

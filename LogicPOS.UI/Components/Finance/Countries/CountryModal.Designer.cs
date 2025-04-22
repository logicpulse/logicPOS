
using Gtk;
using LogicPOS.Utility;
using System.Drawing;
using System.Collections.Generic;
using LogicPOS.UI.Components.InputFields;

namespace LogicPOS.UI.Components.Modals
{
    internal partial class CountryModal
    {
        public override Size ModalSize => new Size(500, 500);
        public override string ModalTitleResourceName => "window_title_edit_dialog_configuration_country";

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtDesignation.Entry);
            SensitiveFields.Add(_txtCapital.Entry);
            SensitiveFields.Add(_txtCurrency.Entry);
            SensitiveFields.Add(_txtCode2.Entry);
            SensitiveFields.Add(_txtCode3.Entry);
            SensitiveFields.Add(_txtCurrencyCode.Entry);
            SensitiveFields.Add(_txtFiscalNumberRegex.Entry);
            SensitiveFields.Add(_txtZipCodeRegex.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkDisabled);
        }

        protected override void AddValidatableFields()
        {
            switch (_modalMode)
            {
                case EntityEditionModalMode.Insert:
                    ValidatableFields.Add(_txtDesignation);
                    break;
                case EntityEditionModalMode.Update:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtOrder);
                    ValidatableFields.Add(_txtCode);
                    break;
            }
        }

        private VBox CreateDetailsTab()
        {
            var tab1 =  new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            if(_modalMode != EntityEditionModalMode.Insert)
            {
                tab1.PackStart(_txtOrder.Component, false, false, 0);
                tab1.PackStart(_txtCode.Component, false, false, 0);
            }

            tab1.PackStart(_txtDesignation.Component, false, false, 0);
            tab1.PackStart(_txtCapital.Component, false, false, 0);
            tab1.PackStart(_txtCurrency.Component, false, false, 0);

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                tab1.PackStart(_checkDisabled, false, false, 0);
            }
          
            return tab1;
        }

        private VBox CreateOthersTab()
        {
            var tab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            tab2.PackStart(_txtCode2.Component, false, false, 0);
            tab2.PackStart(_txtCode3.Component, false, false, 0);
            tab2.PackStart(_txtCurrencyCode.Component, false, false, 0);
            tab2.PackStart(_txtFiscalNumberRegex.Component, false, false, 0);
            tab2.PackStart(_txtZipCodeRegex.Component, false, false, 0);

            return tab2;
        }

        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            yield return (CreateDetailsTab(), GeneralUtils.GetResourceByName("global_record_main_detail"));
            yield return (CreateOthersTab(), GeneralUtils.GetResourceByName("global_others"));
            yield return (CreateNotesTab(), GeneralUtils.GetResourceByName("global_notes"));
        }
    }
}

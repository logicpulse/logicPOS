using System;
using System.Collections.Generic;
using System.Drawing;
using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CurrencyModal
    {
        public override Size ModalSize => new Size(600, 500);
        public override string ModalTitleResourceName => "window_title_edit_configurationcurrency";

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtDesignation.Entry);
            SensitiveFields.Add(_txtAcronym.Entry);
            SensitiveFields.Add(_txtExchangeRate.Entry);
            SensitiveFields.Add(_txtEntity.Entry);
            SensitiveFields.Add(_txtEntity.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkDisabled);
        }

        protected override void AddValidatableFields()
        {
            switch (_modalMode)
            {
                case EntityEditionModalMode.Insert:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtAcronym);
                    ValidatableFields.Add(_txtExchangeRate);
                    break;
                case EntityEditionModalMode.Update:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtOrder);
                    ValidatableFields.Add(_txtCode);
                    ValidatableFields.Add(_txtAcronym);
                    ValidatableFields.Add(_txtExchangeRate);
                    break;
            }
        }

        private VBox CreateDetailsTab()
        {
            var details = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                details.PackStart(_txtOrder.Component, false, false, 0);
                details.PackStart(_txtCode.Component, false, false, 0);
            }

            details.PackStart(_txtDesignation.Component, false, false, 0);
            details.PackStart(_txtAcronym.Component, false, false, 0);
            details.PackStart(_txtEntity.Component, false, false, 0);
            details.PackStart(_txtExchangeRate.Component, false, false, 0);


            if (_modalMode != EntityEditionModalMode.Insert)
            {
                details.PackStart(_checkDisabled, false, false, 0);
            }

            return details;
        }

        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            yield return (CreateDetailsTab(), GeneralUtils.GetResourceByName("global_record_main_detail"));
            yield return (CreateNotesTab(), GeneralUtils.GetResourceByName("global_notes"));
        }
    }
}

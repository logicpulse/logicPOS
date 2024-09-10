using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;


namespace LogicPOS.UI.Components.Modals
{
    public partial class PaymentMethodModal
    {
        public override Size ModalSize => new Size(500, 400);
        public override string ModalTitleResourceName => "global_payment_method";

        #region Components
        private TextBox _txtOrder = TextBoxes.CreateOrderField();
        private TextBox _txtCode = TextBoxes.CreateCodeField();
        private TextBox _txtDesignation = TextBoxes.CreateDesignationField();
        private TextBox _txtToken = new TextBox("global_ConfigurationPaymentMethod_Token");
        private TextBox _txtAcronym= new TextBox("global_ConfigurationPaymentMethod_Acronym");
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
        #endregion

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtDesignation.Entry);
            SensitiveFields.Add(_txtToken.Entry);
            SensitiveFields.Add(_txtAcronym.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkDisabled);
        }

        protected override void AddValidatableFields()
        {

            switch (_modalMode)
            {
                case EntityModalMode.Insert:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtAcronym);
                    _txtAcronym.Entry.Sensitive=false;
                    _txtToken.Entry.Sensitive = false;

                    break;
                case EntityModalMode.Update:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtAcronym);
                    ValidatableFields.Add(_txtToken);

                    _txtAcronym.Entry.Sensitive = false;
                    _txtToken.Entry.Sensitive = false;
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
            tab1.PackStart(_txtAcronym.Component, false, false, 0);
            tab1.PackStart(_txtToken.Component,false, false, 0);

            if (_modalMode != EntityModalMode.Insert)
            {
                tab1.PackStart(_checkDisabled, false, false, 0);
            }

            return tab1;
        }
    }
}

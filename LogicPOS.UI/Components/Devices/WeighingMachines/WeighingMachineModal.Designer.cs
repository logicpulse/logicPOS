using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Modals
{
    public partial class WeighingMachineModal
    {

        public override Size ModalSize => new Size(500, 600);
        public override string ModalTitleResourceName => "global_ConfigurationWeighingMachine";

        #region Components
        private TextBox _txtOrder = TextBoxes.CreateOrderField();
        private TextBox _txtCode = TextBoxes.CreateCodeField();
        private TextBox _txtDesignation = TextBoxes.CreateDesignationField();
        private TextBox _txtPortName = new TextBox("global_hardware_com_portname", true,true, @"^COM\d+$");
        private TextBox _txtBaudRate = new TextBox("global_hardware_com_baudrate", true, true, @"^(4800|9600)$");
        private TextBox _txtParity = new TextBox("global_hardware_com_parity", true, true, @"^(Even|None|Odd)$");
        private TextBox _txtStopBits = new TextBox("global_hardware_com_stopbits", true, true, @"^(One|Two)$");
        private TextBox _txtDataBits = new TextBox("global_hardware_com_databits", true, true, @"^(7|8|9)$");
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
        #endregion

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtDesignation.Entry);
            SensitiveFields.Add(_txtPortName.Entry);
            SensitiveFields.Add(_txtBaudRate.Entry);
            SensitiveFields.Add(_txtParity.Entry);
            SensitiveFields.Add(_txtStopBits.Entry);
            SensitiveFields.Add(_txtDataBits.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkDisabled);
        }

        protected override void AddValidatableFields()
        {

            switch (_modalMode)
            {
                case EntityModalMode.Insert:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtPortName);
                    ValidatableFields.Add(_txtBaudRate);
                    ValidatableFields.Add(_txtParity);
                    ValidatableFields.Add(_txtStopBits);
                    ValidatableFields.Add(_txtDataBits);
                    
                    break;
                case EntityModalMode.Update:
                    ValidatableFields.Add(_txtOrder);
                    ValidatableFields.Add(_txtCode);
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtPortName);
                    ValidatableFields.Add(_txtBaudRate);
                    ValidatableFields.Add(_txtParity);
                    ValidatableFields.Add(_txtStopBits);
                    ValidatableFields.Add(_txtDataBits);

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
            tab1.PackStart(_txtPortName.Component, false, false, 0);
            tab1.PackStart(_txtBaudRate.Component, false, false, 0);
            tab1.PackStart(_txtParity.Component, false, false, 0);
            tab1.PackStart(_txtStopBits.Component, false, false, 0);
            tab1.PackStart(_txtDataBits.Component, false, false, 0);

            if (_modalMode != EntityModalMode.Insert)
            {
                tab1.PackStart(_checkDisabled, false, false, 0);
            }

            return tab1;
        }

    }
}

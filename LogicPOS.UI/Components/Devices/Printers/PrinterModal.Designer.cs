using Gtk;
using System.Collections.Generic;
using System.Drawing;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using LogicPOS.Api.Entities;
using System;
using System.Drawing.Printing;
using System.CodeDom;


namespace LogicPOS.UI.Components.Modals
{
    public partial class PrinterModal
    {
        public override Size ModalSize => new Size(500, 450);
        public override string ModalTitleResourceName => "dialog_edit_DialogConfigurationPrinters_tab1_label";

        #region Components
        private TextBox _txtOrder = TextBoxes.CreateOrderField();
        private TextBox _txtCode = TextBoxes.CreateCodeField();
        private TextBox _txtNetworkName = new TextBox("global_networkname", true);
        private ComboBox _comboDesignation;
        private Label _labelDesignation => CreateDesignationLabel("global_designation");
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
        private EntityComboBox<PrinterType> _comboPrinterTypes;
        #endregion

        protected override void BeforeDesign()
        {
            InitializePriceTypesComboBox();
            _comboDesignation = CreatePrinterDesignationCombobox();
        }

        private void InitializePriceTypesComboBox()
        {
            var printerTypes = GetPrinterTypes();
            var labelText = GeneralUtils.GetResourceByName("global_printer_type");
            var currentPrinterType = _entity != null ? _entity.Type : null;

            _comboPrinterTypes = new EntityComboBox<PrinterType>(labelText,
                                                             printerTypes,
                                                             currentPrinterType,
                                                             true);
        }


        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_labelDesignation);
            SensitiveFields.Add(_comboDesignation);
            SensitiveFields.Add(_txtNetworkName.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkDisabled);
            SensitiveFields.Add(_comboPrinterTypes.ComboBox);
        }

        protected override void AddValidatableFields()
        {
            switch (_modalMode)
            {
                case EntityModalMode.Insert:
                    ValidatableFields.Add(_txtNetworkName);
                    ValidatableFields.Add(_comboPrinterTypes);
                    break;
                case EntityModalMode.Update:
                    ValidatableFields.Add(_txtNetworkName);
                    ValidatableFields.Add(_comboPrinterTypes);
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
            var detailsTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            if (_modalMode != EntityModalMode.Insert)
            {
                detailsTab.PackStart(_txtOrder.Component, false, false, 0);
                detailsTab.PackStart(_txtCode.Component, false, false, 0);
            }
            detailsTab.PackStart(_comboPrinterTypes.Component, false, false, 0);
            detailsTab.PackStart(_labelDesignation, false, false, 0);
            detailsTab.PackStart(_comboDesignation, false, false, 0);
            detailsTab.PackStart(_txtNetworkName.Component, false, false, 0);
            
            if (_modalMode != EntityModalMode.Insert)
            {
                detailsTab.PackStart(_checkDisabled, false, false, 0);
            }

            return detailsTab;
        }


        private ComboBox CreatePrinterDesignationCombobox()
        {

            TreeIter iter = new TreeIter();
            ListStore listStore = new ListStore(typeof(string));
            if (PrinterSettings.InstalledPrinters != null)
            {
                foreach (var printer in PrinterSettings.InstalledPrinters)
                {
                    listStore.AppendValues(printer.ToString());
                }
            }
            CellRendererText cellRendererText = new CellRendererText();
            ComboBox comboBox = new ComboBox(listStore);
            comboBox.PackStart(cellRendererText, true);
            comboBox.AddAttribute(cellRendererText, "text", 0);

            iter = getCurrentDesignation(comboBox);

            return comboBox;
        }

        private TreeIter getCurrentDesignation(ComboBox comboBox)
        {
            TreeIter iter;
            if (comboBox.Model.GetIterFirst(out iter))
            {
                do
                {
                    var designation = (string)comboBox.Model.GetValue(iter, 0);
                    if (designation == _entity.Designation)
                    {
                        comboBox.SetActiveIter(iter);
                        break;
                    }

                } while (comboBox.Model.IterNext(ref iter));
            }

            return iter;
        }

        private Label CreateDesignationLabel(string labelResourceName)
                {
                var label = new Label(GeneralUtils.GetResourceByName(labelResourceName));
                label.SetAlignment(0.0F, 0.0F);
                return label;
            }
    }
}

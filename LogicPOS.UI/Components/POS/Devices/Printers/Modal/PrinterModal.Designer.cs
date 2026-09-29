using Gtk;
using System.Collections.Generic;
using System.Drawing;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using LogicPOS.Api.Entities;
using System;
using System.Drawing.Printing;
using System.CodeDom;
using LogicPOS.Globalization;


namespace LogicPOS.UI.Components.Modals
{
    public partial class PrinterModal
    {
        private Notebook _notebook;
        private VBox _thermalTab;
        private string _thermalTabTitle;

        public override Size ModalSize => new Size(500, 560);
        public override string ModalTitleResourceName => "dialog_edit_DialogConfigurationPrinters_tab1_label";

        protected override void Initialize()
        {
            InitializePrinterTypesComboBox();
            _comboDesignation = CreatePrinterDesignationCombobox();
            ApplyThermalCharsDefaults();
            _comboPrinterTypes.ComboBox.Changed += OnPrinterTypeChanged;
        }

        private void OnPrinterTypeChanged(object sender, EventArgs e)
        {
            UpdateThermalTabVisibility();
        }

        private bool IsThermalTypeSelected() =>
            _comboPrinterTypes?.SelectedEntity?.ThermalPrinter == true;

        private void UpdateThermalTabVisibility()
        {
            if (_notebook == null || _thermalTab == null)
            {
                return;
            }

            var showThermal = IsThermalTypeSelected();
            var pageIndex = _notebook.PageNum(_thermalTab);

            if (showThermal && pageIndex < 0)
            {
                // Insert before the notes tab (always last).
                var insertAt = Math.Max(1, _notebook.NPages - 1);
                _notebook.InsertPage(_thermalTab, new Label(_thermalTabTitle), insertAt);
                _thermalTab.ShowAll();
            }
            else if (!showThermal && pageIndex >= 0)
            {
                if (_notebook.CurrentPage == pageIndex)
                {
                    _notebook.CurrentPage = 0;
                }

                _notebook.RemovePage(pageIndex);
            }

            SyncThermalValidation(showThermal);
        }

        private void SyncThermalValidation(bool isThermal)
        {
            if (_modalMode == EntityEditionModalMode.View)
            {
                return;
            }

            if (isThermal)
            {
                ValidatableFields.Add(_txtThermalMaxCharsPerLineNormal);
                ValidatableFields.Add(_txtThermalMaxCharsPerLineNormalBold);
                ValidatableFields.Add(_txtThermalMaxCharsPerLineSmall);
            }
            else
            {
                ValidatableFields.Remove(_txtThermalMaxCharsPerLineNormal);
                ValidatableFields.Remove(_txtThermalMaxCharsPerLineNormalBold);
                ValidatableFields.Remove(_txtThermalMaxCharsPerLineSmall);
            }
        }

        private void ApplyThermalCharsDefaults()
        {
            if (_entity == null || _entity.ThermalMaxCharsPerLineNormal.GetValueOrDefault() <= 0)
            {
                _txtThermalMaxCharsPerLineNormal.Text = LogicPOS.Api.Entities.Printer.DefaultThermalMaxCharsPerLineNormal.ToString();
            }
            if (_entity == null || _entity.ThermalMaxCharsPerLineNormalBold.GetValueOrDefault() <= 0)
            {
                _txtThermalMaxCharsPerLineNormalBold.Text = LogicPOS.Api.Entities.Printer.DefaultThermalMaxCharsPerLineNormalBold.ToString();
            }
            if (_entity == null || _entity.ThermalMaxCharsPerLineSmall.GetValueOrDefault() <= 0)
            {
                _txtThermalMaxCharsPerLineSmall.Text = LogicPOS.Api.Entities.Printer.DefaultThermalMaxCharsPerLineSmall.ToString();
            }
        }

        private void InitializePrinterTypesComboBox()
        {
            var printerTypes = GetPrinterTypes();
            var labelText = LocalizedString.Instance["global_printer_type"];
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
            SensitiveFields.Add(_txtThermalMaxCharsPerLineNormal.Entry);
            SensitiveFields.Add(_txtThermalMaxCharsPerLineNormalBold.Entry);
            SensitiveFields.Add(_txtThermalMaxCharsPerLineSmall.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkDisabled);
            SensitiveFields.Add(_comboPrinterTypes.ComboBox);
        }

        protected override void AddValidatableFields()
        {
            switch (_modalMode)
            {
                case EntityEditionModalMode.Insert:
                case EntityEditionModalMode.Update:
                    ValidatableFields.Add(_comboPrinterTypes);
                    ValidatableFields.Add(_txtNetworkName);
                    SyncThermalValidation(IsThermalTypeSelected());
                    break;
            }
        }

        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            // Thermal tab is managed separately in CreateNoteBook / UpdateThermalTabVisibility.
            yield return (CreateDetailsTab(), LocalizedString.Instance["global_record_main_detail"]);
            yield return (CreateNotesTab(), LocalizedString.Instance["global_notes"]);
        }

        protected override Notebook CreateNoteBook()
        {
            _notebook = new Notebook { BorderWidth = 3 };
            _thermalTab = CreateThermalTab();
            _thermalTabTitle = LocalizedString.Instance["global_printer_thermal_printer"];

            foreach (var tab in CreateTabs())
            {
                _notebook.AppendPage(tab.Page, new Label(tab.Title));
            }

            if (IsThermalTypeSelected())
            {
                _notebook.InsertPage(_thermalTab, new Label(_thermalTabTitle), 1);
            }

            return _notebook;
        }

        private VBox CreateDetailsTab()
        {
            var detailsTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                detailsTab.PackStart(_txtOrder.Component, false, false, 0);
                detailsTab.PackStart(_txtCode.Component, false, false, 0);
            }
            detailsTab.PackStart(_comboPrinterTypes.Component, false, false, 0);
            detailsTab.PackStart(_labelDesignation, false, false, 0);
            detailsTab.PackStart(_comboDesignation, false, false, 0);
            detailsTab.PackStart(_txtNetworkName.Component, false, false, 0);
            
            if (_modalMode != EntityEditionModalMode.Insert)
            {
                detailsTab.PackStart(_checkDisabled, false, false, 0);
            }

            return detailsTab;
        }

        private VBox CreateThermalTab()
        {
            var thermalTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };
            thermalTab.PackStart(_txtThermalMaxCharsPerLineNormal.Component, false, false, 0);
            thermalTab.PackStart(_txtThermalMaxCharsPerLineNormalBold.Component, false, false, 0);
            thermalTab.PackStart(_txtThermalMaxCharsPerLineSmall.Component, false, false, 0);
            return thermalTab;
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
                    if (_entity != null)
                    {
                        if (designation == _entity.Designation)
                        {
                            comboBox.SetActiveIter(iter);
                            break;
                        }
                    }

                } while (comboBox.Model.IterNext(ref iter));
            }

            return iter;
        }

        private Label CreateDesignationLabel(string labelResourceName)
                {
                var label = new Label(LocalizedString.Instance[labelResourceName]);
                label.SetAlignment(0.0F, 0.0F);
                return label;
            }
    }
}

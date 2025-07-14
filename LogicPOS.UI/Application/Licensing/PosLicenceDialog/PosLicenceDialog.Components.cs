using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.UI.Buttons;

namespace LogicPOS.UI.Components.Licensing
{
    internal partial class PosLicenceDialog
    {
        private HBox _hboxMain;
        private EntryBoxValidation _entryBoxHardwareId;
        private EntryBoxValidation _entryBoxSoftwareKey;
        private readonly IconButtonWithText _buttonRegister;
        private readonly IconButtonWithText _buttonContinue;
        private readonly IconButtonWithText _buttonClose;
        public EntryBoxValidation EntryBoxName { get; set; }
        public EntryBoxValidation EntryBoxCompany { get; set; }
        public EntryBoxValidation EntryBoxFiscalNumber { get; set; }
        public EntryBoxValidation EntryBoxAddress { get; set; }
        public EntryBoxValidation EntryBoxEmail { get; set; }
        public EntryBoxValidation EntryBoxPhone { get; set; }
        public ListComboBox ComboBoxCountry { get; set; }
    }
}

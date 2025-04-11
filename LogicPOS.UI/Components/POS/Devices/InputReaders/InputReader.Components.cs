using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class InputReaderModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtReaderSizes = TextBox.Simple("global_input_reader_card_reader_sizes", true, true, @"^\d+(,\d+)*$");
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
    }
}

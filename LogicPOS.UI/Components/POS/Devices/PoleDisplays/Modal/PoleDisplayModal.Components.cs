using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PoleDisplayModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtVendorId = TextBox.Simple("global_pole_display_vid", true, true, @"^0x[\d]{4}$");
        private TextBox _txtProductId = TextBox.Simple("global_pole_display_pid", true, true, @"^0x[\d]{4}$");
        private TextBox _txtEndpoint = TextBox.Simple("global_pole_display_endpoint", true, true, @"^Ep[\d]{2}$");
        private TextBox _txtCOMPort = TextBox.Simple("global_pole_display_com_port", true, true, @"^COM[\d]{1}$");
        private TextBox _txtCodeTable = TextBox.Simple("global_pole_display_codetable", true, true, @"^0x[\d]{2}$");
        private TextBox _txtCharsPerLine = TextBox.Simple("global_pole_display_number_of_characters_per_line", true, true, @"^[\d]+$");
        private TextBox _txtStandByInSeconds = TextBox.Simple("global_pole_display_goto_stand_by_in_seconds", true, true, @"^[\d]+$");
        private TextBox _txtStandByLine1 = TextBox.Simple("global_pole_display_stand_by_line_no");
        private TextBox _txtStandByLine2 = TextBox.Simple("global_pole_display_stand_by_line_no");
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
     }
}

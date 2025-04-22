using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class HolidayModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtDescription = TextBox.Simple("global_description");
        private TextBox _txtDay = TextBox.Simple("global_day", true, true, "^(0?[1-9]|[12][0-9]|3[01])$");
        private TextBox _txtMonth = TextBox.Simple("global_month", true, true, "^(0?[1-9]|1[0-2])$");
        private TextBox _txtYear = TextBox.Simple("global_year", true, true, "^[0-9]+$");
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
    }
}

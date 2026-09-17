using LogicPOS.UI.Components.InputFields;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PreferenceParameterModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtToken = TextBox.Simple("global_token");
        private PreferenceParameterInputField _field;
        
    }
}

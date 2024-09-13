using LogicPOS.UI.Components.InputFields.Validation;

namespace LogicPOS.UI.Components.InputFields
{
    public static class TextBoxes
    {
        public static TextBox CreateOrderField() => new TextBox("global_record_order", true, true, "^[0-9]+$");
        public static TextBox CreateCodeField() => new TextBox("global_record_code", true, true, RegularExpressions.Alphanumeric);
        public static TextBox CreateDesignationField() => new TextBox("global_designation", true);
    }
}

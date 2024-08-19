namespace LogicPOS.UI.Components.InputFieds
{
    public static class TextBoxes
    {
        public static TextBox CreateOrderField() => new TextBox("global_record_order", true, true, "^[0-9]+$");
        public static TextBox CreateCodeField() => new TextBox("global_record_code", true, true,"^[a-zA-Z0-9]+$");
        public static TextBox CreateDesignationField() => new TextBox("global_designation", true);
    }
}

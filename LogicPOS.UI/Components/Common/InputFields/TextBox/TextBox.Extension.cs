using Gtk;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.InputFields.Validation;
using System;

namespace LogicPOS.UI.Components.InputFields
{
    public partial class TextBox
    {
        public static TextBox CreateOrderField() => Simple("global_record_order", true, true, "^[0-9]+$");
        public static TextBox CreateCodeField() => Simple("global_record_code", false, true, RegularExpressions.AlfaNumericArticleCode);
        public static TextBox CreateDesignationField() => Simple("global_designation", true);

        public static HBox CreateHbox(params TextBox[] textBoxes)
        {
            var hbox = new HBox(true, 2);

            foreach (var textBox in textBoxes)
            {
                hbox.PackStart(textBox.Component, false, true, 0);
            }
            return hbox;
        }

        public static TextBox Simple(string labelResource,
                                     bool isRequired = false,
                                     bool isValidatable = false,
                                     string regex = null) =>
            new TextBox(null,
                        LocalizedString.Instance[labelResource],
                        isRequired,
                        isValidatable,
                        regex,
                        includeSelectButton: false,
                        includeClearButton: false,
                        style: TextBoxStyle.Simple);

    }
}

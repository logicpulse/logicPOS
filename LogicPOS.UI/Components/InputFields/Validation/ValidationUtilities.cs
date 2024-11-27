using Gtk;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.BackOffice.Windows;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.InputFields.Validation
{
    public static class ValidationUtilities
    {
        public static void ShowValidationErrors(IEnumerable<IValidatableField> fields,
                                                Window sourceWindow = null)
        {
            var invalidFields = string.Join(", ",
                                            fields.Where(field => field.IsValid() == false)
                                                             .Select(field => field.FieldName));

            CustomAlerts.Error(sourceWindow ?? BackOfficeWindow.Instance)
                        .WithSize(new Size(500, 340))
                        .WithTitleResource("window_title_dialog_validation_error")
                        .WithMessage(string.Format(GeneralUtils.GetResourceByName("dialog_message_field_validation_error"),
                                                  invalidFields))
                        .ShowAlert();
        }
    }
}

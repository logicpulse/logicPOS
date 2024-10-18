using Gtk;
using logicpos;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.InputFields.Validation
{
    public static class ValidationUtilities
    {
        public static void ShowValidationErrors(IEnumerable<IValidatableField> fields)
        {
            var invalidFields = string.Join(", ",
                                            fields.Where(field => field.IsValid() == false)
                                                             .Select(field => field.FieldName));

            Utils.ShowMessageBox(GlobalApp.BackOffice,
                                 DialogFlags.DestroyWithParent | DialogFlags.Modal,
                                 new Size(500, 500),
                                 MessageType.Error,
                                 ButtonsType.Ok,
                                 GeneralUtils.GetResourceByName("window_title_dialog_validation_error"),
                                 string.Format(GeneralUtils.GetResourceByName("dialog_message_field_validation_error"),
                                               invalidFields));
        }
    }
}

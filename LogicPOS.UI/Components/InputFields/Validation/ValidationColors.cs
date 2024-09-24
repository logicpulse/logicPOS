using Gtk;
using LogicPOS.UI.Extensions;
using System.Drawing;

namespace LogicPOS.UI.Components.InputFields.Validation
{
    public class ValidationColors
    {
        public Color ValidFontColor { get; set; }
        public Color ValidBackgroundColor { get; set; }
        public Color InvalidFontColor { get; set; }
        public Color InvalidBackgroundColor { get; set; }

        public void UpdateComponentFontColor(Widget component, bool isValid)
        {
            if (isValid)
            {
                component.ModifyFg(StateType.Normal, ValidFontColor.ToGdkColor());
                component.ModifyFg(StateType.Active, ValidFontColor.ToGdkColor());

                component.ModifyText(StateType.Normal, ValidFontColor.ToGdkColor());
                component.ModifyText(StateType.Active, ValidFontColor.ToGdkColor());
                return;
            }

            component.ModifyFg(StateType.Normal, InvalidFontColor.ToGdkColor());
            component.ModifyFg(StateType.Active, InvalidFontColor.ToGdkColor());

            component.ModifyText(StateType.Normal, InvalidFontColor.ToGdkColor());
            component.ModifyText(StateType.Active, InvalidFontColor.ToGdkColor());
        }

        public void UpdateComponentBackgroundColor(Widget component, bool isValid)
        {
            if (isValid)
            {
                component.ModifyBase(StateType.Normal, ValidBackgroundColor.ToGdkColor());
                return;
            }

            component.ModifyBase(StateType.Normal, InvalidBackgroundColor.ToGdkColor());
        }

        public void UpdateComponent(
            Widget component,
            bool isValid,
            bool updateFontColor = true,
            bool updateBackgroundColor = true)

        {
            if (updateFontColor)
            {
                UpdateComponentFontColor(component, isValid);
            }

            if (updateBackgroundColor)
            {
                UpdateComponentBackgroundColor(component, isValid);
            }
        }


        public readonly static ValidationColors Default = new ValidationColors
        {
            ValidFontColor = Settings.AppSettings.Instance.colorEntryValidationValidFont,
            ValidBackgroundColor = Settings.AppSettings.Instance.colorEntryValidationValidBackground,
            InvalidFontColor = Settings.AppSettings.Instance.colorEntryValidationInvalidFont,
            InvalidBackgroundColor = Settings.AppSettings.Instance.colorEntryValidationInvalidBackground
        };


    }
}

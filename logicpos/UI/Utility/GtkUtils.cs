using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets.Entrys;
using logicpos.Extensions;
using LogicPOS.Settings;

namespace LogicPOS.UI
{
    public static class GtkUtils
    {
        public static EventBox CreateMinimizeButton()
        {
            string minimizeWindowIconLocation = GtkSettings.DefaultMinimizeWindowIconLocation;

            Gdk.Pixbuf pixBuffer = new Gdk.Pixbuf(minimizeWindowIconLocation);
            Gtk.Image buttonImage = new Gtk.Image(pixBuffer);
            EventBox button = new EventBox();
            button.WidthRequest = pixBuffer.Width;
            button.HeightRequest = pixBuffer.Height;
            button.Add(buttonImage);

            return button;
        }

        public static void UpdateWidgetColorsAfterValidation(Widget widget, bool validated, params Label[] labels)
        {
            widget = widget.GetType() == typeof(EntryBoxValidationMultiLine)
                ? (widget as EntryBoxValidationMultiLine).EntryMultiline.TextView
                : widget;

            var validColor = ColorSettings.ValidTextBoxColor.StringToGdkColor();
            var invalidColor = ColorSettings.InvalidTextBoxColor.StringToGdkColor();
            var validBackgroundColor = ColorSettings.ValidTextBoxBackgroundColor.StringToGdkColor();
            var invalidBackgroundColor = ColorSettings.InvalidTextBoxBackgroundColor.StringToGdkColor();

            if (validated)
            {
                widget.ModifyText(StateType.Normal, validColor);
                widget.ModifyText(StateType.Active, validColor);
                widget.ModifyBase(StateType.Normal, validBackgroundColor);

                foreach (var label in labels)
                {
                    label.ModifyFg(StateType.Normal, validColor);
                }

                return;
            }

            widget.ModifyText(StateType.Normal, invalidColor);
            widget.ModifyText(StateType.Active, invalidColor);
            widget.ModifyBase(StateType.Normal, invalidBackgroundColor);

            foreach (var label in labels)
            {
                label.ModifyFg(StateType.Normal, invalidColor);
            }
        }
    }
}

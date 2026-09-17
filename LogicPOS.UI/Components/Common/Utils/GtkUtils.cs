using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets.Entrys;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Settings;

namespace LogicPOS.UI
{
    public static class GtkUtils
    {
        
        public static EventBox CreateMinimizeButton()
        {
            string minimizeWindowIconLocation = AppSettings.Paths.Images + @"Icons\Windows\icon_window_window_minimize.png";

            Gdk.Pixbuf pixBuffer = new Gdk.Pixbuf(minimizeWindowIconLocation);
            Gtk.Image buttonImage = new Gtk.Image(pixBuffer);
            EventBox button = new EventBox();
            button.WidthRequest = pixBuffer.Width;
            button.HeightRequest = pixBuffer.Height;
            button.Add(buttonImage);

            return button;
        }

        public static void UpdateWidgetColorsAfterValidation(Widget widget,
                                                             bool validated,
                                                             params Label[] labels)
        {
            widget = widget.GetType() == typeof(EntryBoxValidationMultiLine)
                ? (widget as EntryBoxValidationMultiLine).EntryMultiline.TextView
                : widget;

            var validColor = AppSettings.Colors.ValidTextBoxColor.ToGdkColor();
            var invalidColor = AppSettings.Colors.InvalidTextBoxColor.ToGdkColor();
            var validBackgroundColor = AppSettings.Colors.ValidTextBoxBackgroundColor.ToGdkColor();
            var invalidBackgroundColor = AppSettings.Colors.InvalidTextBoxBackgroundColor.ToGdkColor();

            if (validated)
            {
                widget.ModifyText(StateType.Normal, validColor);
                widget.ModifyText(StateType.Active, validColor);
                widget.ModifyBase(StateType.Normal, validBackgroundColor);
                 
                foreach (Label label in labels)
                {
                    if(label != null)
                    {
                        label.ModifyFg(StateType.Normal, validColor);
                    }
                }

                return;
            }

            widget.ModifyText(StateType.Normal, invalidColor);
            widget.ModifyText(StateType.Active, invalidColor);
            widget.ModifyBase(StateType.Normal, invalidBackgroundColor);

            foreach (Label label in labels)
            {
                if (label != null)
                {
                    label.ModifyFg(StateType.Normal, invalidColor);
                }
            }
        }
    }
}

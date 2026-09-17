using Gtk;
using LogicPOS.UI.Extensions;
using System.Drawing;

namespace LogicPOS.UI.Buttons
{
    public class CustomButton : Button
    {
        public EventBox BgEventBox { get; set; }
        public ButtonSettings ButtonSettings { get; private set; }

        public CustomButton(ButtonSettings settings)
        {
            Name = settings.Name;
            BorderWidth = 1;
            Relief = ReliefStyle.Half;
            CanFocus = false;
            ButtonSettings = settings;
        }

        public void Initialize()
        {
            WidthRequest = ButtonSettings.ButtonSize.Width;
            HeightRequest = ButtonSettings.ButtonSize.Height;

            BgEventBox = new EventBox();
            SetBackgroundColor(ButtonSettings.BackgroundColor, BgEventBox);

            if (ButtonSettings.Widget != null)
            {
                BgEventBox.Add(ButtonSettings.Widget);
            }

            Add(BgEventBox);
            ShowAll();
        }

        public void UpdateWidget(Widget widget)
        {
            if (ButtonSettings.Widget != null)
            {
                BgEventBox.Remove(ButtonSettings.Widget);
                BgEventBox.Add(widget);
            }
            ShowAll();
        }

        public void SetBackgroundColor(Color color,
                                       EventBox eventBox = null)
        {
            eventBox = eventBox ?? BgEventBox;

            if (color == Color.Transparent)
            {
                eventBox.VisibleWindow = false;
                return;
            }

            Color normal = color;
            Color prelight = normal.Lighten();
            Color active = prelight.Lighten();
            Color insensitive = normal.Darken();
            Color selected = Color.FromArgb(125, 0, 0);

            eventBox.ModifyBg(StateType.Normal, normal.ToGdkColor());
            eventBox.ModifyBg(StateType.Selected, selected.ToGdkColor());
            eventBox.ModifyBg(StateType.Prelight, prelight.ToGdkColor());
            eventBox.ModifyBg(StateType.Active, active.ToGdkColor());
            eventBox.ModifyBg(StateType.Insensitive, insensitive.ToGdkColor());
        }
    }
}

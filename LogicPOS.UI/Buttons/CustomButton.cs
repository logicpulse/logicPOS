using Gtk;
using LogicPOS.UI.Extensions;
using System;
using System.Drawing;

namespace LogicPOS.UI.Buttons
{
    public class CustomButton : Button
    {
        protected EventBox _backgroundColorEventBox;
        public string Token { get; set; }
        public Guid CurrentButtonId { get; set; }
        protected ButtonSettings _settings;

        public CustomButton(ButtonSettings settings)
        {
            Name = settings.Name;
            BorderWidth = 1;
            Relief = ReliefStyle.Half;
            CanFocus = false;
            _settings = settings;
        }

        public void Initialize()
        {
            WidthRequest = _settings.ButtonSize.Width;
            HeightRequest = _settings.ButtonSize.Height;

            _backgroundColorEventBox = new EventBox();
            SetBackgroundColor(_settings.BackgroundColor, _backgroundColorEventBox);

            if (_settings.Widget != null) _backgroundColorEventBox.Add(_settings.Widget);
            Add(_backgroundColorEventBox);

            ShowAll();
        }

        public void SetBackgroundColor(Color color,
                                       EventBox eventBox)
        {
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

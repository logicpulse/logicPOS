using Gtk;
using LogicPOS.UI.Extensions;
using System;
using System.Drawing;

namespace LogicPOS.UI.Buttons
{
    public class CustomButton : Button
    {
        public EventBox BackgroundColorEventBox { get; set; }
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

            BackgroundColorEventBox = new EventBox();
            SetBackgroundColor(_settings.BackgroundColor, BackgroundColorEventBox);

            if (_settings.Widget != null) BackgroundColorEventBox.Add(_settings.Widget);
            Add(BackgroundColorEventBox);
          
            ShowAll();
        }

        public void SetBackgroundColor(Color color,
                                       EventBox eventBox = null)
        {
            eventBox = eventBox ?? BackgroundColorEventBox;

            if (color == Color.Transparent)
            {
                eventBox.VisibleWindow = false;
                return;
            }

            eventBox.VisibleWindow = true;

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

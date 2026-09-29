using Gtk;
using System.Drawing;

namespace LogicPOS.UI.Buttons
{
    public class ButtonSettings
    {
        public string Name { get; set; }
        public Color BackgroundColor { get; set; } = Color.Transparent;
        public Widget Widget { get; set; }
        public Size ButtonSize { get; set; }
        public string Text { get; set; }
        public string Font { get; set; }
        public int FontSize { get; set; }
        public Color FontColor { get; set; }
        public string Icon { get; set; }
        public Size IconSize { get; set; }
        public bool LeftImage { get; set; }
        public bool Logged { get; set; }
        public string Image { get; set; }
        public string Overlay { get; set; }

        public ButtonSettings Clone()
        {
            return new ButtonSettings
            {
                Name = Name,
                BackgroundColor = BackgroundColor,
                Widget = Widget,
                ButtonSize = ButtonSize,
                Text = Text,
                Font = Font,
                FontSize = FontSize,
                FontColor = FontColor,
                Icon = Icon,
                IconSize = IconSize,
                LeftImage = LeftImage,
                Logged = Logged,
                Image = Image,
                Overlay = Overlay
            };
        }
    }
}

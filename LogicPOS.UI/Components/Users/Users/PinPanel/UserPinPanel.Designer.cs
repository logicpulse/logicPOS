using Gtk;
using logicpos;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using System.Drawing;

namespace LogicPOS.UI.Components
{
    public partial class UserPinPanel
    {
        private string Font { get; } = AppSettings.Instance.fontNumberPadPinButtonKeysTextAndLabel;
        private  string StatusLabelFont { get; } = "bold 12";
        private Color ButtonFontColor { get; } = Color.White;
        private Size _btnSize;
        private readonly Label _labelStatus;
        private Gtk.Table _table;
        public EventBox Eventbox { get; set; } = new EventBox() { VisibleWindow = false };
        public ValidatableTextBox TxtPin { get; set; }
        public TextButton BtnOk { get; set; }
        public IconButton BtnResetPassword { get; set; }
        public TextButton BtnQuit { get; set; }

        private TextButton CreateNumberedButton(string number)
        {
            return new TextButton(
                new ButtonSettings
                {
                    Name = "buttonUserId",
                    Text = number,
                    Font = Font,
                    FontColor = ButtonFontColor,
                    ButtonSize = _btnSize
                });
        }
    }
}

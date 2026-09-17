using LogicPOS.Api.Features.Common;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Settings;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Common.Menus
{
    public sealed class MenuButton<TEntity> where TEntity : ApiEntity
    {
        public TEntity Entity { get; set; }
        public CustomButton Button { get; set; }
        public static int MaxCharsPerButtonLabel => AppSettings.Instance.PosBaseButtonMaxCharsPerLabel;
        private static int ButtonFontSize { get; } = Convert.ToInt16(AppSettings.Instance.FontPosBaseButtonSize);
        private static readonly string _buttonOverlay = (AppSettings.Instance.UseImageOverlay) ? AppSettings.Paths.Images + @"Buttons\Pos\button_overlay.png" : null;

        public MenuButton(TEntity entity, CustomButton button)
        {
            Entity = entity;
            Button = button;
        }

        public static CustomButton CreateButton(string buttonName, string label, string imagePath, Size _buttonSize)
        {

            if (label.Length > MaxCharsPerButtonLabel)
            {
                label = label.Substring(0, MaxCharsPerButtonLabel) + ".";
            }

            return new ImageButton(
                new ButtonSettings
                {
                    Name = buttonName,
                    Text = label,
                    FontSize = ButtonFontSize,
                    Image = imagePath,
                    Overlay = _buttonOverlay,
                    ButtonSize = _buttonSize,
                });
        }
    }
}

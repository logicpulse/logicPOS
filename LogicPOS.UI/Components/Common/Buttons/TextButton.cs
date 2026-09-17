using LogicPOS.UI.Extensions;
using System.Drawing;

namespace LogicPOS.UI.Buttons
{
    public class TextButton : CustomButton
    {
        public Gtk.Label ButtonLabel { get; set; }

        public TextButton(ButtonSettings settings,
                          bool initialize = true) :
            base(settings)
        {
            ButtonLabel = new Gtk.Label(settings.Text);
            ChangeFont(settings.Font, settings.FontColor);
            ButtonSettings.Widget = ButtonLabel;

            if(initialize)
            {
                Initialize();
            }
        }


        public void SetFont(string font)
        {
            Pango.FontDescription fontDesc = Pango.FontDescription.FromString(font);
            ButtonLabel.ModifyFont(fontDesc);
        }

        public void ChangeFont(string font,
                               Color color)
        {
            SetFont(font);

            Color colNormal = color;
            Color colPrelight = colNormal.Lighten();
            Color colActive = colPrelight.Lighten();
            Color colInsensitive = colNormal.Lighten();
            Color colSelected = Color.FromArgb(125, 0, 0);

            ButtonLabel.ModifyFg(Gtk.StateType.Normal, colNormal.ToGdkColor());
            ButtonLabel.ModifyFg(Gtk.StateType.Prelight, colPrelight.ToGdkColor());
            ButtonLabel.ModifyFg(Gtk.StateType.Active, colActive.ToGdkColor());
            ButtonLabel.ModifyFg(Gtk.StateType.Insensitive, colInsensitive.ToGdkColor());
            ButtonLabel.ModifyFg(Gtk.StateType.Selected, colSelected.ToGdkColor());
        }
    }
}

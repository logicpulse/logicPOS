using Gtk;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Settings;
using System.Drawing;

namespace LogicPOS.UI.Buttons
{
    public class XAccordionChildButton
    {
        public Button Button { get; private set; }

        public XAccordionChildButton(string text)
        {
            Button = CreateComponent(text);
        }

        private Button CreateComponent(string text)
        {
            HBox component = new HBox(false, 0);

            component.PackStart(CreateLabel(text), true, true, 0);
            var button = new Button(component);
            button.Name = "AccordionChildButton";

            if (BackOfficeWindow.ScreenSize.Height <= 800)
            {
                button.HeightRequest = 23;
            }
            else
            {
                button.HeightRequest = 25;
            }

            return button;
        }

        public static Label CreateLabel(string text)
        {
            var label = new Label(text);
            Color colNormal = "61, 61, 61".StringToColor();
            Color colPrelight = colNormal.Lighten();
            Color colActive = colPrelight.Lighten();
            Color colInsensitive = colNormal.Darken();
            Color colSelected = Color.FromArgb(125, 0, 0);

            string _fontPosBackOfficeParent = AppSettings.Instance.FontPosBackOfficeParent;
            string _fontPosBackOfficeChild = AppSettings.Instance.FontPosBackOfficeChild;
            string _fontPosBackOfficeParentLowRes = AppSettings.Instance.FontPosBackOfficeParentLowRes;
            string _fontPosBackOfficeChildLowRes = AppSettings.Instance.FontPosBackOfficeChildLowRes;

            Pango.FontDescription fontPosBackOfficeparentLowRes = Pango.FontDescription.FromString(_fontPosBackOfficeParentLowRes);
            Pango.FontDescription fontPosBackOfficeParent = Pango.FontDescription.FromString(_fontPosBackOfficeParent);
            Pango.FontDescription fontPosBackOfficeChildLowRes = Pango.FontDescription.FromString(_fontPosBackOfficeChildLowRes);
            Pango.FontDescription fontPosBackOfficeChild = Pango.FontDescription.FromString(_fontPosBackOfficeChild);


            if (BackOfficeWindow.ScreenSize.Height <= 800)
            {
                label.ModifyFont(fontPosBackOfficeChildLowRes);
            }
            else
            {
                label.ModifyFont(fontPosBackOfficeChild);
            }

            label.ModifyFg(StateType.Normal, colNormal.ToGdkColor());
            label.ModifyFg(StateType.Prelight, colPrelight.ToGdkColor());
            label.ModifyFg(StateType.Active, colActive.ToGdkColor());
            label.ModifyFg(StateType.Insensitive, colInsensitive.ToGdkColor());
            label.ModifyFg(StateType.Selected, colSelected.ToGdkColor());

            label.SetAlignment(0.0f, 0.5f);

            return label;
        }
    }
}

using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI.Extensions;
using System.Drawing;

namespace LogicPOS.UI.Components.Documents.CreateDocument.Fields
{
    public partial class DocumentPaymentMethodsBox
    {
        private VBox CreateScrolledWindow()
        {
            var verticalLayout = new VBox(false, 2);
            var swindow = new ScrolledWindow();
            swindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            swindow.ModifyBg(StateType.Normal, Color.White.ToGdkColor());
            swindow.ShadowType = ShadowType.None;
            swindow.AddWithViewport(Container);
            verticalLayout.PackStart(swindow, true, true, 0);
            verticalLayout.PackStart(LabelTotal, false, false, 0);

            return verticalLayout;
        }

        private Label CreateLabel(string labelText)
        {
            var label = new Label(labelText);
            label.SetAlignment(0.0F, 0.0F);
            label.ModifyFont(Pango.FontDescription.FromString(AppSettings.Instance.fontEntryBoxLabel));
            return label;
        }
    }
}

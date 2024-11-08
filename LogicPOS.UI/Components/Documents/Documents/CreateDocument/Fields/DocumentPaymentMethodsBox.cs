using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Documents.Documents.AddDocument;
using LogicPOS.Settings;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.Documents.CreateDocument.Fields
{
    public class DocumentPaymentMethodsBox : IValidatableField
    {
        public List<DocumentPaymentMethodField> Fields { get; } = new List<DocumentPaymentMethodField>();
        public VBox Container { get; } = new VBox(false, 5) { BorderWidth = (uint)5 };
        public Widget Component { get; private set; }
        public Window SourceWindow { get; set; }
        public Label LabelTotal { get; set; }

        public string FieldName => GeneralUtils.GetResourceByName("global_payment_method");

        public DocumentPaymentMethodsBox(Window sourceWindow)
        {
            SourceWindow = sourceWindow;
            LabelTotal = CreateLabel("Total:");
            Component = CreateScrolledWindow();
            AddDocumentPaymentMethodField();
        }

        private void UpdateTotal()
        {
            LabelTotal.Text = $"Total: {GetTotal()}";
        }

        private decimal GetTotal()
        {
            return Fields.Where(f => f.IsValid()).Sum(f => decimal.Parse(f.TxtAmount.Text));
        }

        private void BtnRemovePaymentMethod_Clicked(DocumentPaymentMethodField field)
        {
            if (Fields.Count < 2)
            {
                return;
            }

            Container.Remove(field.Component);
            Fields.Remove(field);
            UpdateTotal();
        }

        private void AddDocumentPaymentMethodField()
        {
            if (Fields.Any() && Fields.Last().TxtPaymentMethod.SelectedEntity == null)
            {
                return;
            }

            var field = new DocumentPaymentMethodField(SourceWindow);
            field.TxtAmount.Entry.Changed += (sender, e) => UpdateTotal();
            field.TxtPaymentMethod.Entry.Changed += (sender, e) => UpdateTotal();
            field.OnRemove += BtnRemovePaymentMethod_Clicked;
            field.OnAdd += () => AddDocumentPaymentMethodField();
            Container.PackStart(field.Component, false, false, 0);
            field.Component.ShowAll();
            Fields.Add(field);
        }

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

        public bool IsValid()
        {
            return Fields.All(f => f.IsValid());
        }

        public IEnumerable<AddDocumentPaymentMethodDto> GetPaymentMethods()
        {
            return Fields.Select(f => new AddDocumentPaymentMethodDto
            {
                PaymentMethodId = (f.TxtPaymentMethod.SelectedEntity as ApiEntity).Id,
                Amount = decimal.Parse(f.TxtAmount.Text)
            });
        }
    }
}

using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Documents.Documents.AddDocument;
using LogicPOS.UI.Components.InputFields.Validation;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocument.Fields
{
    public partial class DocumentPaymentMethodsBox : IValidatableField
    {
        public DocumentPaymentMethodsBox(Window sourceWindow)
        {
            SourceWindow = sourceWindow;
            LabelTotal = CreateLabel("Total:");
            Component = CreateScrolledWindow();
            AddDocumentPaymentMethodField();
        }

        private void UpdateTotalLabel()
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
            UpdateTotalLabel();
        }

        private void AddDocumentPaymentMethodField()
        {
            if (Fields.Any() && Fields.Last().TxtPaymentMethod.SelectedEntity == null)
            {
                return;
            }

            var field = new DocumentPaymentMethodField(SourceWindow);
            field.TxtAmount.Entry.Changed += (sender, e) => UpdateTotalLabel();
            field.TxtPaymentMethod.Entry.Changed += (sender, e) => UpdateTotalLabel();
            field.OnRemove += BtnRemovePaymentMethod_Clicked;
            field.OnAdd += () => AddDocumentPaymentMethodField();
            Container.PackStart(field.Component, false, false, 0);
            field.Component.ShowAll();
            Fields.Add(field);
        }
        public void UpdateDocumentTotal(decimal total)
        {
            if (Fields.Count == 1)
            {
                Fields.First().TxtAmount.Text = total.ToString();
            }

            UpdateTotalLabel();
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

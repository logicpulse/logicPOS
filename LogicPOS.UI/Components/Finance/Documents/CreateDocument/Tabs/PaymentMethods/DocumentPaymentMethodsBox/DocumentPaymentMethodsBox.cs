using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Documents.Documents.AddDocument;
using LogicPOS.UI.Components.Finance.PaymentMethods;
using LogicPOS.UI.Components.InputFields.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using DocumentPaymentMethod = LogicPOS.Api.Features.Documents.Documents.AddDocument.DocumentPaymentMethod;

namespace LogicPOS.UI.Components.Documents.CreateDocument.Fields
{
    public partial class DocumentPaymentMethodsBox : IValidatableField
    {
        public DocumentPaymentMethodsBox(Window sourceWindow)
        {
            SourceWindow = sourceWindow;
            LabelTotal = CreateLabel("Total:");
            Component = CreateScrolledWindow();
            AddPaymentMethod();
        }

        private void UpdateTotalLabel()
        {
            LabelTotal.Text = $"Total: {GetTotal():F2}";
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

        public void AddPaymentMethod(Guid? id = null, decimal? amount = null)
        {
            if (Fields.Any() && Fields.Last().TxtPaymentMethod.SelectedEntity == null)
            {
                return;
            }

            PaymentMethod paymentMethod = null;

            if (id != null)
            {
                paymentMethod = PaymentMethodsService.GetPaymentMethodById(id.Value);
            }

            var field = new DocumentPaymentMethodField(SourceWindow);
            field.TxtAmount.Text = amount?.ToString("0.00") ?? "0.00";
            field.TxtAmount.Entry.Changed += (sender, e) => UpdateTotalLabel();
            field.TxtPaymentMethod.Entry.Changed += (sender, e) => UpdateTotalLabel();
            field.TxtPaymentMethod.Text = paymentMethod?.Designation;
            field.TxtPaymentMethod.SelectedEntity = paymentMethod;
            field.OnRemove += BtnRemovePaymentMethod_Clicked;
            field.OnAdd += () => AddPaymentMethod();
            Container.PackStart(field.Component, false, false, 0);
            field.Component.ShowAll();
            Fields.Add(field);

            UpdateTotalLabel();
        }

        public void Clear()
        {
            foreach (var field in Fields)
            {
                Container.Remove(field.Component);
            }
            Fields.Clear();
            UpdateTotalLabel();
        }

        public void UpdateDocumentTotal(decimal total)
        {
            if (Fields.Count == 1)
            {
                Fields.First().TxtAmount.Text = total.ToString("0.00");
            }

            UpdateTotalLabel();
        }

        public bool IsValid()
        {
            return Fields.All(f => f.IsValid());
        }

        public IEnumerable<DocumentPaymentMethod> GetPaymentMethods()
        {
            return Fields.Select(f => new DocumentPaymentMethod
            {
                PaymentMethodId = (f.TxtPaymentMethod.SelectedEntity as ApiEntity).Id,
                Amount = decimal.Parse(f.TxtAmount.Text)
            });
        }
    }
}

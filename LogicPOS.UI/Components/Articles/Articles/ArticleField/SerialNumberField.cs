using Gtk;
using LogicPOS.Api.Features.Articles.GetArticleChildren;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Errors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components
{
    public class SerialNumberField : IValidatableField
    {
        public List<SerialNumberSelectionField> Children { get; private set; } = new List<SerialNumberSelectionField>();

        public TextBox TxtSerialNumber = TextBox.Simple("global_serial_number",false);
        public VBox Component { get; private set; } = new VBox(false, 2);

        public string FieldName => LocalizedString.Instance["global_serial_number"];

        public SerialNumberField()
        {
            Component.PackStart(TxtSerialNumber.Component, false, false, 0);
        }

        public void LoadArticleChildren(Guid parentArticleId)
        {
            var result = DependencyInjection.Mediator.Send(new GetArticleChildrenQuery(parentArticleId)).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return;
            }

            foreach (var child in result.Value)
            {
                Children.Add(new SerialNumberSelectionField(child.Article));
            }

            PresentChildFields();
        }

        private void PresentChildFields()
        {
            foreach (var field in Children)
            {
                Component.PackStart(field.Component, false, true, 0);
            }
        }

        public bool IsValid()
        {
            var result = TxtSerialNumber.IsValid();

            if (Children.Any()) {
                result = result &&  Children.All(f => f.IsValid());
            }

            return result;
        }
    }
}

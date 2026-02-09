using LogicPOS.Api.Features.Articles.GetArticleChildren;
using LogicPOS.Api.Features.Articles.StockManagement.GetUniqueArticleChildren;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Errors;
using System;
using System.Linq;

namespace LogicPOS.UI.Components
{
    public partial class SerialNumberField : IValidatableField
    {
        public string FieldName => LocalizedString.Instance["global_serial_number"];

        public SerialNumberField()
        {
            Component.PackStart(TxtSerialNumber.Component, false, false, 0);
        }

        public void LoadArticleChildren(Guid articleId)
        {
            var result = DependencyInjection.Mediator.Send(new GetArticleChildrenQuery(articleId)).Result;

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

        public void LoadUniqueArticleChildren(Guid uniqueArticleId)
        {
            var result = DependencyInjection.Mediator.Send(new GetUniqueArticleChildrenQuery(uniqueArticleId)).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return;
            }

            foreach (var child in result.Value)
            {
                var field = new SerialNumberSelectionField(child.Article);
                field.TxtSerialNumber.Text = child.SerialNumber;
                field.UniqueArticelId = child.Id;
                Children.Add(field);
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

            if (Children.Any())
            {
                result = result && Children.All(f => f.IsValid());
            }

            return result;
        }
    }
}

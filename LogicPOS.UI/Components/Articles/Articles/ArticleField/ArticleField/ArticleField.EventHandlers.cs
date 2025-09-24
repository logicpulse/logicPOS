using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System.Linq;

namespace LogicPOS.UI.Components.InputFields
{
    public partial class ArticleField
    {
        private void BtnSelect_Clicked(object sender, System.EventArgs e)
        {
            var page = new ArticlesPage(null, PageOptions.SelectionPageOptions);
            var selectArticleModal = new EntitySelectionModal<ArticleViewModel>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectArticleModal.Run();
            selectArticleModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                Article = page.GetSelectedArticle();
                ShowEntity();
            }
        }

        private void UpdateSerialNumbersComponents()
        {
            _serialNumberFields.ForEach(f => _serialNumberFieldsContainer.Remove(f.Component));
            _serialNumberFields.Clear();

            int quantity = 0;

            if (int.TryParse(TxtQuantity.Text, out quantity) == false)
            {
                return;
            }

            if (quantity <= 0)
            {
                return;
            }

            for (int i = 0; i < quantity; i++)
            {
                var field = new SerialNumberField();
                if (Article != null && Article.IsComposed)
                {
                    field.LoadArticleChildren(Article.Id);
                }
                _serialNumberFields.Add(field);
                _serialNumberFieldsContainer.PackStart(field.Component, false, false, 10);
            }

            _serialNumberFields.ForEach(f =>
            {
                f.TxtSerialNumber.IsValidFunction = SerialNumberIsUnique;

            });

            Component.ShowAll();
        }

        private bool SerialNumberIsUnique(string serialNumber)
        {
            return _serialNumberFields.Select(f => f.TxtSerialNumber.Text).Count(s => s == serialNumber) == 1;
        }
    }
}

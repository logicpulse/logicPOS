using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.StockManagement.AddStockMovement;
using LogicPOS.Api.Features.Warehouses.GetAllWarehouses;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogicPOS.UI.Components.InputFields
{
    public partial class ArticleField : IValidatableField
    {
        public Article Article { get; set; }
        public string FieldName => Label.Text;
        public event Action<ArticleField, Article> OnRemove;
        public event System.Action OnAdd;
        private readonly bool _isUniqueArticle;
        public ArticleField(Article article = null,
                            decimal quantity = 0,
                            bool isUniqueArticle = false)
        {
            _isUniqueArticle = isUniqueArticle;
            Article = article;
            TxtQuantity.Text = quantity.ToString();
            Label.SetAlignment(0, 0.5f);
            InitializeButtons();

            if (isUniqueArticle)
            {
                _locationField = new WarehouseSelectionField();
            }

            PackComponents();
            AddEventHandlers();
            UpdateValidationColors();
            ShowEntity();
        }

        private void ShowEntity()
        {
            if (Article == null)
            {
                return;
            }

            TxtCode.Text = Article.Code;
            TxtDesignation.Text = Article.Designation;
            TxtQuantity.Text = "1";

            if (_isUniqueArticle)
            {
                UpdateSerialNumbersComponents();
            }
        }

        private void UpdateValidationColors()
        {
            ValidationColors.Default.UpdateComponentFontColor(Label, IsValid());
            ValidationColors.Default.UpdateComponentBackgroundColor(TxtDesignation, Article != null);
            ValidationColors.Default.UpdateComponentBackgroundColor(TxtCode, Article != null);
            ValidationColors.Default.UpdateComponentBackgroundColor(TxtQuantity, QuantityIsValid());
        }

        private bool QuantityIsValid()
        {
            return Regex.IsMatch(TxtQuantity.Text, RegularExpressions.Quantity);
        }

        private void AddEventHandlers()
        {
            BtnRemove.Clicked += (s, e) => OnRemove?.Invoke(this, Article);
            BtnAdd.Clicked += (s, e) => OnAdd?.Invoke();
            TxtQuantity.Changed += (s, e) => UpdateValidationColors();
            BtnSelect.Clicked += BtnSelect_Clicked;

            if (_isUniqueArticle)
            {
                TxtQuantity.Changed += (s, e) =>
                {
                    UpdateSerialNumbersComponents();
                    Component.ShowAll();
                };
            }

        }

        private void BtnSelect_Clicked(object sender, System.EventArgs e)
        {
            var page = new ArticlesPage(null, PageOptions.SelectionPageOptions);
            var selectArticleModal = new EntitySelectionModal<Article>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectArticleModal.Run();
            selectArticleModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                Article = page.SelectedEntity;
                ShowEntity();
            }
        }

        public bool IsValid()
        {
            var result = (Article != null) && QuantityIsValid();

            if (_isUniqueArticle)
            {
                result = result && _locationField.IsValid() && _serialNumberFields.All(f => f.IsValid());
            }

            return result;
        }

        public IEnumerable<StockMovementItem> GetNonLocalizedStockMovementItems()
        {
            yield return new StockMovementItem
            {
                ArticleId = Article.Id,
                Quantity = decimal.Parse(TxtQuantity.Text)
            };
        }

        public IEnumerable<StockMovementItem> GetLocalizedStockMovementItems()
        {
            foreach (var serialNumberField in _serialNumberFields)
            {
                var item = new StockMovementItem
                {
                    ArticleId = Article.Id,
                    Quantity = decimal.Parse(TxtQuantity.Text),
                    SerialNumber = serialNumberField.TxtSerialNumber.Text,
                    WarehouseLocationId = WarehouseLocationId,
                    Price = Price
                };

                if (string.IsNullOrWhiteSpace(serialNumberField.TxtSerialNumber.Text) == false && Article.IsComposed)
                {
                    item.ChildUniqueArticles = serialNumberField.Children.Select(c => c.UniqueArticelId);
                }

                yield return item;
            }
        }

        private decimal Price => string.IsNullOrEmpty(TxtPrice.Text) ? 0 : decimal.Parse(TxtPrice.Text);

        private Guid? WarehouseLocationId => _locationField?.LocationField.SelectedEntity?.Id;
    }
}

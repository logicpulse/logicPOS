using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.StockManagement.AddStockMovement;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.InputFields.Validation;
using System;
using System.Collections.Generic;
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
        public object SelectedEntity { get; set; }
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

        public ArticleField WithDesignationAutoCompletion(List<(object Entity, string Text)> completionSource)
        {
            TxtDesignation.Completion = new EntryCompletion();
            var listStore = new ListStore(typeof(object), typeof(string));
            foreach (var item in completionSource)
            {
                listStore.AppendValues(item.Entity, item.Text);
            }
            TxtDesignation.Completion.Model = listStore;
            TxtDesignation.Completion.TextColumn = 1;
            TxtDesignation.Completion.PopupCompletion = true;
            TxtDesignation.Completion.InlineCompletion = false;
            TxtDesignation.Completion.PopupSingleMatch = true;
            TxtDesignation.Completion.InlineSelection = false;

            TxtDesignation.Completion.MatchFunc = (comp, key, iter) =>
            {
                string value = (string)comp.Model.GetValue(iter, 1);
                return value.Trim().IndexOf(key.Trim(), StringComparison.OrdinalIgnoreCase) >= 0;
            };

            TxtDesignation.Completion.MatchSelected += Completion_MatchSelected;
            return this;
        }

        public ArticleField WithCodeAutoCompletion(List<(object Entity, string Text)> completionSource)
        {
            TxtCode.Completion = new EntryCompletion();
            var listStore = new ListStore(typeof(object), typeof(string));
            foreach (var item in completionSource)
            {
                listStore.AppendValues(item.Entity, item.Text);
            }
            TxtCode.Completion.Model = listStore;
            TxtCode.Completion.TextColumn = 1;
            TxtCode.Completion.PopupCompletion = true;
            TxtCode.Completion.InlineCompletion = false;
            TxtCode.Completion.PopupSingleMatch = true;
            TxtCode.Completion.InlineSelection = false;

            TxtCode.Completion.MatchFunc = (comp, key, iter) =>
            {
                string value = (string)comp.Model.GetValue(iter, 1);
                return value.Trim().IndexOf(key.Trim(), StringComparison.OrdinalIgnoreCase) >= 0;
            };

            TxtCode.Completion.MatchSelected += Completion_MatchSelected;
            return this;
        }

        [GLib.ConnectBefore]
        private void Completion_MatchSelected(object o, MatchSelectedArgs args)
        {
            object entity = args.Model.GetValue(args.Iter, 0);
            SelectedEntity = entity;
            Article=ArticlesService.GetArticlebById((entity as ArticleViewModel).Id);
            ShowEntity();
            UpdateValidationColors();
            OnCompletionSelected?.Invoke(entity);
        }

  

        private decimal Price => string.IsNullOrEmpty(TxtPrice.Text) ? 0 : decimal.Parse(TxtPrice.Text);

        private Guid? WarehouseLocationId => _locationField?.LocationField.SelectedEntity?.Id;
    }
}

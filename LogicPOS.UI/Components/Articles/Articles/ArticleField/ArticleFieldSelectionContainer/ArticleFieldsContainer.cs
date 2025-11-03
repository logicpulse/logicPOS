using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.StockManagement.AddStockMovement;
using LogicPOS.UI.Components.Enums;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Articles
{
    public partial class ArticleFieldsContainer : IValidatableField
    {
        public string FieldName => GeneralUtils.GetResourceByName("global_article");

        public ArticleFieldsContainer(ArticlesBoxMode mode = ArticlesBoxMode.Article)
        {
            _mode = mode;
            Component = CreateScrolledWindow();
            AddArticle();
        }

        private void BtnRemoveArticle_Clicked(ArticleField field, Article article)
        {
            if (Fields.Count < 2)
            {
                return;
            }

            Container.Remove(field.Component);
            Fields.Remove(field);
        }

        private void AddArticle(Article article = null,
                                decimal quantity = 0)
        {
            var lastSlotIsNull = Fields.Any() && Fields.Last().Article == null;

            if (lastSlotIsNull)
            {
                return;
            }

            var field = new ArticleField(article, quantity, isUniqueArticle: _mode == ArticlesBoxMode.StockManagement);
            field.WithAutoCompletion(ArticlesService.AutocompleteLines);
            field.OnRemove += BtnRemoveArticle_Clicked;
            field.OnAdd += () => AddArticle();

            SetFieldStyle(field);

            Container.PackStart(field.Component, false, false, 0);
            Component.ShowAll();
            Fields.Add(field);
        }

     
        public void AddArticleChildren(IEnumerable<Api.Features.Articles.GetArticleChildren.ArticleChild> children)
        {
            if (children.Count() == 0)
            {
                return;
            }

            Clear();

            foreach (var child in children)
            {
                var field = new ArticleField(child.Article, child.Quantity);
                field.OnRemove += BtnRemoveArticle_Clicked;
                field.OnAdd += () => AddArticle();
                Container.PackStart(field.Component, false, false, 0);
                field.Component.ShowAll();
                Fields.Add(field);
            }
        }

        private void Clear()
        {
            Fields.ForEach(f => Container.Remove(f.Component));
            Fields.Clear();
        }

        public bool IsValid()
        {
            return Fields.All(f => f.IsValid());
        }

        public IEnumerable<StockMovementItem> GetStockMovementItems()
        {
            List<StockMovementItem> items = new List<StockMovementItem>();
            foreach (ArticleField field in Fields)
            {
                if (_mode == ArticlesBoxMode.StockManagement)
                {
                    items.AddRange(field.GetFullStockMovementItems());
                    continue;
                }

                items.AddRange(field.GetSimpleStockMovementItems());
            }

            return items;
        }

        public IEnumerable<ArticleChild> GetArticleChildren()
        {
            return Fields.Select(f => new ArticleChild
            {
                ArticleId = f.Article.Id,
                Quantity = decimal.Parse(f.TxtQuantity.Text)
            });
        }
    }
}

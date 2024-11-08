using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Articles
{
    public class AddArticlesBox : IValidatableField
    {
        public List<ArticleField> Fields { get; } = new List<ArticleField>();
        public VBox Container { get; } = new VBox(false, 5) { BorderWidth = (uint)5 };
        public ScrolledWindow Component { get; private set; }

        public string FieldName => GeneralUtils.GetResourceByName("global_article");

        public AddArticlesBox()
        {
            Component = CreateScrolledWindow();
            AddArticleField();
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

        private void AddArticleField(Article article = null)
        {
            if (Fields.Any() && Fields.Last().Article == null)
            {
                return;
            }

            var field = new ArticleField(article);
            field.OnRemove += BtnRemoveArticle_Clicked;
            field.OnAdd += () => AddArticleField();
            Container.PackStart(field.Component, false, false, 0);
            field.Component.ShowAll();
            Fields.Add(field);
        }

        private ScrolledWindow CreateScrolledWindow()
        {
            var swindow = new ScrolledWindow();
            swindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            swindow.ModifyBg(StateType.Normal, Color.White.ToGdkColor());
            swindow.ShadowType = ShadowType.None;
            swindow.AddWithViewport(Container);
            return swindow;
        }

        public bool IsValid()
        {
            return Fields.All(f => f.IsValid());
        }

        public IEnumerable<ArticleStock> GetArticlesStocks()
        {
            return Fields.Select(f => new ArticleStock
            {
                Id = f.Article.Id,
                Quantity = decimal.Parse(f.TxtQuantity.Text)
            });
        }
    }
}

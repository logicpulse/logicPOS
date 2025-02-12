using Gtk;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UpdateUniqueArticleModal : EntityEditionModal<ArticleHistory>
    {
        public UpdateUniqueArticleModal(ArticleHistory entity) : base(EntityEditionModalMode.Update, entity)
        {
        }

        protected override void AddEntity()
        {
            throw new System.NotImplementedException();
        }

        protected override void AddValidatableFields()
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            throw new System.NotImplementedException();
        }

        protected override void ShowEntityData()
        {
            throw new System.NotImplementedException();
        }

        protected override void UpdateEntity()
        {
            throw new System.NotImplementedException();
        }
    }
}

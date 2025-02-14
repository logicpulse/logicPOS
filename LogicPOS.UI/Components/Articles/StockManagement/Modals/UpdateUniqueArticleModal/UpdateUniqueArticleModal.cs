using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;

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

using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.Api.Features.Articles.StockManagement.UpdateUniqueArticle;
using LogicPOS.UI.Errors;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UpdateUniqueArticleModal : EntityEditionModal<ArticleHistory>
    {
        public UpdateUniqueArticleModal(ArticleHistory entity) : base(EntityEditionModalMode.Update, entity)
        {
        }

        protected override void AddEntity() { }

        protected override void ShowEntityData()
        {
            SerialNumberField.TxtSerialNumber.Text = _entity.WarehouseArticle.SerialNumber;
            if (_entity.WarehouseArticle.Status != Api.Enums.ArticleSerialNumberStatus.Available)
            {
                SerialNumberField.TxtSerialNumber.Entry.Sensitive = false;
            }
            SerialNumberField.LoadUniqueArticleChildren(_entity.WarehouseArticle.Id);
        }

        protected override void UpdateEntity()
        {
            ExecuteUpdateCommand(CreateUpdateCommand());
        }

        private UpdateUniqueArticleCommand CreateUpdateCommand()
        {
            var childUniqueArticles = SerialNumberField.Children.Select(x => x.UniqueArticelId).ToList();
            return new UpdateUniqueArticleCommand
            {
                Id = _entity.WarehouseArticle.Id,
                SerialNumber = SerialNumberField.TxtSerialNumber.Text,
                ChildUniqueArticles = childUniqueArticles
            };
        }
    }
}

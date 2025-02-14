using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.StockManagement.ChangeArticleLocation;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ChangeArticleLocationModal : EntityEditionModal<WarehouseArticle>
    {
        public ChangeArticleLocationModal(WarehouseArticle entity) : base(EntityEditionModalMode.Update, entity)
        {
        }

        protected override void AddEntity() { }

        protected override void ShowEntityData()
        {
            _txtArticle.SelectedEntity = _entity.Article;
            _txtArticle.Text = _entity.Article.Designation;
            _txtSerialNumber.Text = _entity.SerialNumber;
            _txtQuantity.Text = _entity.Quantity.ToString();
        }

        protected override void UpdateEntity()
        {
            var command = new ChangeArticleLocationCommand()
            {
                WarehouseArticleId = _entity.Id,
                LocationId = _locationField.LocationField.SelectedEntity.Id
            };

            ExecuteUpdateCommand(command);
        }
    }
}
